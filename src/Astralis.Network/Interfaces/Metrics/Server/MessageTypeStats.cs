using System;
using System.Collections.Concurrent;
using System.Threading;
using Astralis.Network.Interfaces.Metrics.Types;

namespace Astralis.Network.Interfaces.Metrics.Server;

public class MessageTypeStats
{
    private long _sent;
    private long _received;
    private DateTime _lastSent = DateTime.MinValue;
    private DateTime _lastReceived = DateTime.MinValue;
    private readonly ConcurrentDictionary<string, long> _sessionStats = new();

    public long Sent => _sent;
    public long Received => _received;
    public DateTime LastSent => _lastSent;
    public DateTime LastReceived => _lastReceived;

    public void TrackMessage(MessageDirection direction, string sessionId = null)
    {
        if (direction == MessageDirection.Incoming)
        {
            Interlocked.Increment(ref _received);
            _lastReceived = DateTime.UtcNow;
        }
        else
        {
            Interlocked.Increment(ref _sent);
            _lastSent = DateTime.UtcNow;
        }

        if (sessionId != null)
        {
            _sessionStats.AddOrUpdate(sessionId, 1, (_, count) => count + 1);
        }
    }
}
