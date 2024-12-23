using System.Reactive.Linq;
using Astralis.Core.Interfaces.EventBus;
using Astralis.Core.Interfaces.Services;
using Astralis.Network.Data.Events.Clients;
using Astralis.Network.Data.Events.Network;
using Astralis.Network.Data.Internal;
using Astralis.Network.Data.Session;
using Astralis.Network.Interfaces.Listeners;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Metrics.Server;
using Astralis.Network.Interfaces.Metrics.Types;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets.Base;
using Astralis.Server.Data.Configs;
using Humanizer;
using LiteNetLib;
using Serilog;

namespace Astralis.Server.Services;

public class NetworkServer
    : INetworkServer,
        IEventBusListener<SendMessageEvent>,
        IEventBusListener<SendMessagesEvent>,
        IEventBusListener<RegisterNetworkListenerEvent<INetworkMessage>>
{
    public bool IsRunning { get; private set; }

    private readonly NetworkServerMetrics _metrics;


    public IObservable<NetworkMetricsSnapshot> MetricsObservable { get; set; }


    private readonly ILogger _logger = Log.ForContext<INetworkServer>();

    private readonly IMessageDispatcherService _messageDispatcherService;

    private readonly IMessageChannelService _messageChannelService;

    private readonly NetworkServerConfig _config;

    private readonly IEventBusService _eventBusService;

    private readonly IMessageParserWriterService _messageParserWriterService;

    private readonly INetworkMessageFactory _networkMessageFactory;

    private readonly INetworkSessionService _networkSessionService;

    private readonly CancellationTokenSource _readMessageCancellationTokenSource = new();

    private readonly Task _poolEventTask;

    private readonly Task _writeMessageTask;

    private readonly EventBasedNetListener _serverListener = new();

    private readonly PeriodicTimer _poolTimer = new(TimeSpan.FromMilliseconds(15));
    private readonly NetManager _netServer;

    public NetworkServer(
        IMessageDispatcherService messageDispatcherService, IMessageParserWriterService messageParserWriterService,
        INetworkSessionService networkSessionService, IEventBusService eventBusService,
        IMessageChannelService messageChannelService, INetworkMessageFactory networkMessageFactory,
        NetworkServerMetrics metrics,
        NetworkServerConfig config
    )
    {
        _config = config;
        _metrics = metrics;
        _networkMessageFactory = networkMessageFactory;
        _messageChannelService = messageChannelService;
        _eventBusService = eventBusService;
        _messageDispatcherService = messageDispatcherService;
        _messageParserWriterService = messageParserWriterService;
        _networkSessionService = networkSessionService;

        _serverListener.ConnectionRequestEvent += OnConnectionRequested;
        _serverListener.PeerDisconnectedEvent += OnPeerDisconnection;
        _serverListener.NetworkReceiveEvent += OnNetworkEvent;

        _eventBusService.Subscribe<SendMessageEvent>(this);
        _eventBusService.Subscribe<SendMessagesEvent>(this);
        _eventBusService.Subscribe<RegisterNetworkListenerEvent<INetworkMessage>>(this);

        _netServer = new NetManager(_serverListener);

        _poolEventTask = ServerPoolEvents();

        _writeMessageTask = WriteMessageChannel();

        MetricsObservable = _metrics.MetricsObservable.AsObservable();

        MetricsObservable.Sample(TimeSpan.FromSeconds(10)).Subscribe(LogMetricsSnapshot);
    }

    public void LogMetricsSnapshot(NetworkMetricsSnapshot snapshot)
    {
        _logger.Information(
            "[Network] Conn: {Active}/{Peak} | " +
            "Msgs: {Sent}/{Rcv} | " +
            "Bytes: {BytesSent}/{BytesRcv} | " +
            "Peers: {PeerCount} | " +
            "Events: {EventCount}",
            snapshot.ServerStats.ActiveConnections,
            snapshot.ServerStats.PeakConnections,
            snapshot.ServerStats.TotalMessagesSent,
            snapshot.ServerStats.TotalMessagesReceived,
            snapshot.ServerStats.TotalBytesSent.Bytes(),
            snapshot.ServerStats.TotalBytesReceived.Bytes(),
            snapshot.PeerStats.Count,
            snapshot.RecentEvents.Length
        );
    }

    private async Task ServerPoolEvents()
    {
        _logger.Information("Starting server event loop");

        while (await _poolTimer.WaitForNextTickAsync())
        {
            if (_netServer.IsRunning)
            {
                _netServer.PollEvents();
            }
        }
    }

    private async Task WriteMessageChannel()
    {
        _logger.Information("Starting write message channel");
        while (!_readMessageCancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await foreach (var packet in _messageChannelService
                                   .OutgoingReaderChannel
                                   .ReadAllAsync()
                                   .WithCancellation(_readMessageCancellationTokenSource.Token))
                {
                    if (string.IsNullOrEmpty(packet.SessionId))
                    {
                        await BroadcastMessageAsync(packet.Packet);
                        continue;
                    }

                    var session = _networkSessionService.GetSessionObject(packet.SessionId);
                    var message = await _networkMessageFactory.SerializeAsync(packet.Packet);


                    await session.WriteLock.WaitAsync();
                    await _messageParserWriterService.WriteMessageAsync(
                        session.Peer,
                        session.Writer,
                        (NetworkPacket)message
                    );

                    _metrics.TrackMessage(
                        session.Peer,
                        message.MessageType,
                        MessageDirection.Outgoing,
                        session.Writer.Length
                    );

                    session.WriteLock.Release();
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in write message channel");
                await Task.Delay(1000);
            }
        }

        _logger.Information("Stopping write message channel");
    }

    private void OnNetworkEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _messageParserWriterService.ReadPackets(reader, peer);
    }

    private void OnPeerDisconnection(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _metrics.TrackDisconnection(peer, disconnectInfo);
        _logger.Information("Peer {peerId} disconnected", peer.Id);

        _networkSessionService.RemoveSession(peer.Id.ToString());

        _eventBusService.PublishAsync(new ClientDisconnectedEvent(peer.Id.ToString()));
    }

    private void OnConnectionRequested(ConnectionRequest request)
    {
        _logger.Information("Connection request from {endPoint}", request.RemoteEndPoint);

        var peer = request.Accept();

        _metrics.TrackNewConnection(peer);

        _networkSessionService.AddSession(peer.Id.ToString(), new SessionObject(peer));

        _eventBusService.PublishAsync(new ClientConnectedEvent(peer.Id.ToString(), peer));
    }


    public Task StartAsync()
    {
        _logger.Information("Starting server on port: {Port}", _config.Port);

        _netServer.Start(_config.Port);

        IsRunning = true;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _logger.Information("Stopping server");

        _netServer.Stop();

        IsRunning = false;

        return Task.CompletedTask;
    }

    public void RegisterMessageListener<TMessage>(INetworkMessageListener<TMessage> listener)
        where TMessage : class, INetworkMessage
    {
        _messageDispatcherService.RegisterMessageListener(listener);
    }

    public void RegisterMessageListener<TMessage>(
        Func<string, TMessage, ValueTask> listener
    ) where TMessage : class, INetworkMessage
    {
        _messageDispatcherService.RegisterMessageListener(listener);
    }

    public async ValueTask BroadcastMessageAsync(INetworkMessage message)
    {
        foreach (var sessionId in _networkSessionService.GetSessionIds)
        {
            await SendMessageAsync(new SessionNetworkMessage(sessionId, message));
        }
    }

    public async ValueTask SendMessagesAsync(IEnumerable<SessionNetworkMessage> messages)
    {
        foreach (var messageToSend in messages)
        {
            _messageChannelService.OutgoingWriterChannel.WriteAsync(messageToSend);
        }
    }

    public ValueTask SendMessageAsync(SessionNetworkMessage messages)
    {
        return SendMessagesAsync(new List<SessionNetworkMessage> { messages });
    }

    public ValueTask SendMessageAsync(string sessionId, INetworkMessage message)
    {
        return SendMessageAsync(new SessionNetworkMessage(sessionId, message));
    }


    public void Dispose()
    {
        _metrics.Dispose();
        _readMessageCancellationTokenSource.Dispose();
        _messageDispatcherService.Dispose();
    }

    public async Task OnEventAsync(SendMessageEvent message)
    {
        await SendMessageAsync(new SessionNetworkMessage(message.SessionId, message.Message));
    }

    public async Task OnEventAsync(SendMessagesEvent message)
    {
        await SendMessagesAsync(message.Messages.Select(x => new SessionNetworkMessage(message.SessionId, x)));
    }

    public Task OnEventAsync(RegisterNetworkListenerEvent<INetworkMessage> message)
    {
        RegisterMessageListener(message.Listener);

        return Task.CompletedTask;
    }
}
