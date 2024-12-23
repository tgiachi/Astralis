using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Astralis.Network.Interfaces.Metrics.Types;
using Astralis.Network.Types;
using LiteNetLib;
using Serilog;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class NetworkServerMetrics : IDisposable
{
    private readonly ILogger _logger = Log.ForContext<NetworkServerMetrics>();
    public NetworkServerStats ServerStats { get; } = new();
    private readonly ConcurrentDictionary<string, LiteNetStats> _peerStats = new();
    private readonly ConcurrentDictionary<Type, MessageTypeStats> _messageTypeStats = new();
    private readonly ConcurrentQueue<NetworkEvent> _events = new();
    private const int MaxEvents = 1000;

    private readonly Subject<NetworkMetricsSnapshot> _metricsSubject = new();
    private readonly IDisposable _metricsSubscription;

    public IObservable<NetworkMetricsSnapshot> MetricsObservable => _metricsSubject.AsObservable();


    public NetworkServerMetrics()
    {
        _metricsSubscription = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Select(_ => GetSnapshot())
            .Subscribe(_metricsSubject);
    }

    public void UpdatePeerStats(NetPeer peer)
    {
        var stats = peer.Statistics;
        var peerId = peer.Id.ToString();

        _peerStats.AddOrUpdate(
            peerId,
            new LiteNetStats
            {
                PacketsSent = stats.PacketsSent,
                PacketsReceived = stats.PacketsReceived,
                BytesSent = stats.BytesSent,
                BytesReceived = stats.BytesReceived,
                PacketLoss = stats.PacketLoss,
                RoundTripTime = TimeSpan.FromMilliseconds(peer.Ping)
            },
            (_, existing) =>
            {
                existing.Update(stats, peer.Ping);

                // Check for latency spikes
                if (peer.Ping > 200) // 200ms threshold
                {
                    LogEvent(
                        NetworkEventType.LatencySpike,
                        $"High latency detected: {peer.Ping}ms",
                        peerId
                    );
                }

                // Check for packet loss
                if (stats.PacketLoss > 5) // 5% threshold
                {
                    LogEvent(
                        NetworkEventType.PacketLoss,
                        $"High packet loss detected: {stats.PacketLoss}%",
                        peerId
                    );
                }

                return existing;
            }
        );
    }

    public void TrackMessage(NetPeer peer, NetworkMessageType message, MessageDirection direction, int bytes)
    {
        if (direction == MessageDirection.Incoming)
        {
            ServerStats.TrackReceivedMessage(bytes);
            LogEvent(NetworkEventType.Info, $"Received message {message.GetType().Name} from {peer.Id}", peer.Id.ToString());
        }
        else
        {
            ServerStats.TrackSentMessage(bytes);
            LogEvent(NetworkEventType.Info, $"Sent message {message.GetType().Name} to {peer.Id}", peer.Id.ToString());
        }

        var typeStats = _messageTypeStats.GetOrAdd(message.GetType(), _ => new MessageTypeStats());
        typeStats.TrackMessage(direction, peer.Id.ToString());

        UpdatePeerStats(peer);
    }


    public void TrackNewConnection(NetPeer peer)
    {
        ServerStats.TrackNewConnection();
        LogEvent(
            NetworkEventType.ConnectionStateChanged,
            $"New connection from peer {peer.Id}",
            peer.Id.ToString()
        );
    }

    public void TrackDisconnection(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        ServerStats.TrackDisconnection();
        LogEvent(
            NetworkEventType.ConnectionStateChanged,
            $"Peer {peer.Id} disconnected: {disconnectInfo.Reason}",
            peer.Id.ToString()
        );
    }

    private void LogEvent(NetworkEventType type, string message, string sessionId)
    {
        var evt = new NetworkEvent(type, message, DateTime.UtcNow, sessionId);
        _events.Enqueue(evt);

        while (_events.Count > MaxEvents)
        {
            _events.TryDequeue(out _);
        }

        // Log to serilog based on event type
        switch (type)
        {
            case NetworkEventType.Error:
                _logger.Error("[{SessionId}] {Message}", sessionId, message);
                break;
            case NetworkEventType.Warning:
            case NetworkEventType.LatencySpike:
            case NetworkEventType.PacketLoss:
                _logger.Warning("[{SessionId}] {Message}", sessionId, message);
                break;
            default:
                _logger.Verbose("[{SessionId}] {Message}", sessionId, message);
                break;
        }
    }

    public NetworkMetricsSnapshot GetSnapshot()
    {
        return new NetworkMetricsSnapshot
        {
            ServerStats = new NetworkServerStatsSnapshot
            {
                ActiveConnections = ServerStats.ActiveConnections,
                PeakConnections = ServerStats.PeakConnections,
                TotalMessagesSent = ServerStats.TotalMessagesSent,
                TotalMessagesReceived = ServerStats.TotalMessagesReceived,
                TotalBytesSent = ServerStats.TotalBytesSent,
                TotalBytesReceived = ServerStats.TotalBytesReceived,
                UptimeSeconds = (DateTime.UtcNow - ServerStats.StartTime).TotalSeconds
            },
            PeerStats = _peerStats.ToDictionary(x => x.Key, x => x.Value),
            MessageTypeStats = _messageTypeStats.ToDictionary(
                x => x.Key.Name,
                x => new MessageTypeStatsSnapshot
                {
                    Sent = x.Value.Sent,
                    Received = x.Value.Received,
                    LastSent = x.Value.LastSent,
                    LastReceived = x.Value.LastReceived
                }
            ),
            RecentEvents = _events.ToArray()
        };
    }

    public void Dispose()
    {
        _metricsSubject.Dispose();
        _metricsSubscription.Dispose();
    }
}
