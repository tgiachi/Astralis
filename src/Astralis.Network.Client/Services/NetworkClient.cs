using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Astralis.Network.Client.Interfaces;
using Astralis.Network.Data.Internal;
using Astralis.Network.Encoders;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Interfaces.Services;
using Astralis.Network.Packets.Base;
using Astralis.Network.Services;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace Astralis.Network.Client.Services;

public class NetworkClient : INetworkClient
{
    public event INetworkClient.MessageReceivedEventHandler? MessageReceived;
    public event EventHandler? Connected;

    public bool IsConnected { get; private set; }


    private readonly ILogger _logger = Log.Logger.ForContext<NetworkClient>();
    private readonly EventBasedNetListener _clientListener = new();

    private readonly Subject<INetworkMessage> _messageSubject = new();

    private readonly SemaphoreSlim _writeLock = new(1, 1);

    private readonly NetManager _netManager;

    private bool _connected;


    private readonly INetworkMessageFactory _networkMessageFactory;

    private readonly NetPacketProcessor _netPacketProcessor = new();

    private readonly NetDataWriter writer = new();

    public NetworkClient(List<MessageTypeObject> messageTypes)
    {
        _netManager = new NetManager(_clientListener);
        var messageTypesService = new MessageTypesService(messageTypes);
        _networkMessageFactory = new NetworkMessageFactory(
            messageTypesService,
            new ProtobufDecoder(),
            new ProtobufEncoder()
        );

        _netPacketProcessor.SubscribeReusable<NetworkPacket, NetPeer>(OnReceivePacket);
        _clientListener.NetworkReceiveEvent += OnMessageReceived;
    }

    private async void OnReceivePacket(NetworkPacket packet, NetPeer peer)
    {
        _logger.Debug("Received packet from server type: {Type}", packet.MessageType);

        if (!_connected)
        {
            _connected = true;
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        var message = await _networkMessageFactory.ParseAsync(packet);

        _logger.Debug("Parsed message from server type: {Type}", message.GetType().Name);

        MessageReceived?.Invoke(packet.MessageType, message);

        _messageSubject.OnNext(message);
    }

    private void OnMessageReceived(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _netPacketProcessor.ReadAllPackets(reader, peer);
    }


    public void Connect(string host, int port)
    {
        _netManager.Start();
        _netManager.Connect(host, port, string.Empty);

        IsConnected = true;
    }

    public async Task SendMessageAsync<T>(T message) where T : class, INetworkMessage
    {
        if (!IsConnected)
        {
            _logger.Warning("Dropping message {messageType} as client is not connected", message.GetType().Name);
            return;
        }

        await _writeLock.WaitAsync();

        var packet = (NetworkPacket)(await _networkMessageFactory.SerializeAsync(message));

        _logger.Debug(">> Sending message {messageType}", message.GetType().Name);

        writer.Reset();

        _netPacketProcessor.Write(writer, packet);


        foreach (var peer in _netManager.ConnectedPeerList)
        {
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        _writeLock.Release();
    }

    public IObservable<T> SubscribeToMessage<T>() where T : class, INetworkMessage
    {
        return _messageSubject.OfType<T>();
    }


    public void PoolEvents()
    {
        if (_netManager.IsRunning)
        {
            _netManager.PollEvents();
        }
    }
}
