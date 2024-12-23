using System;
using System.Collections.Concurrent;
using System.Threading;
using LiteNetLib;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class NetworkServerStats
{
    private volatile int _activeConnections;
    private volatile int _peakConnections;
    private long _totalMessagesSent;
    private long _totalMessagesReceived;
    private long _totalBytesSent;
    private long _totalBytesReceived;
    private readonly ConcurrentDictionary<DeliveryMethod, long> _deliveryMethodStats = new();
    public DateTime StartTime { get; } = DateTime.UtcNow;

    public int ActiveConnections => _activeConnections;
    public int PeakConnections => _peakConnections;
    public long TotalMessagesSent => _totalMessagesSent;
    public long TotalMessagesReceived => _totalMessagesReceived;
    public long TotalBytesSent => _totalBytesSent;
    public long TotalBytesReceived => _totalBytesReceived;

    public void TrackNewConnection()
    {
        var current = Interlocked.Increment(ref _activeConnections);
        var peak = _peakConnections;
        while (current > peak)
        {
            Interlocked.CompareExchange(ref _peakConnections, current, peak);
            peak = _peakConnections;
        }
    }

    public void TrackDisconnection() => Interlocked.Decrement(ref _activeConnections);

    public void TrackSentMessage(int bytes)
    {
        Interlocked.Increment(ref _totalMessagesSent);
        Interlocked.Add(ref _totalBytesSent, bytes);
    }

    public void TrackReceivedMessage(int bytes)
    {
        Interlocked.Increment(ref _totalMessagesReceived);
        Interlocked.Add(ref _totalBytesReceived, bytes);
    }

    public void TrackDeliveryMethod(DeliveryMethod method) =>
        _deliveryMethodStats.AddOrUpdate(method, 1, (_, count) => Interlocked.Increment(ref count));
}
