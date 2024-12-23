using Astralis.Core.Interfaces.Events;

namespace Astralis.Network.Data.Events.Sessions;

public class SessionAddedEvent : IAstralisEvent
{
    public string SessionId { get; }

    public SessionAddedEvent(string sessionId)
    {
        SessionId = sessionId;
    }

    public override string ToString()
    {
        return $"SessionAddedEvent: {SessionId}";
    }


}
