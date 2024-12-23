using System;
using Astralis.Network.Interfaces.Metrics.Types;

namespace Astralis.Network.Interfaces.Metrics.Server;

public struct NetworkEvent
{
    public NetworkEventType Type { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }
    public string SessionId { get; }
    public Exception Exception { get; }

    public NetworkEvent(
        NetworkEventType type, string message, DateTime timestamp, string sessionId, Exception exception = null
    )
    {
        Type = type;
        Message = message;
        Timestamp = timestamp;
        SessionId = sessionId;
        Exception = exception;
    }

    public static NetworkEvent CreateError(string message, Exception ex, string sessionId = null) =>
        new(NetworkEventType.Error, message, DateTime.UtcNow, sessionId, ex);

    public static NetworkEvent CreateInfo(string message, string sessionId = null) =>
        new(NetworkEventType.Info, message, DateTime.UtcNow, sessionId);

    public static NetworkEvent CreateWarning(string message, string sessionId = null) =>
        new(NetworkEventType.Warning, message, DateTime.UtcNow, sessionId);
}
