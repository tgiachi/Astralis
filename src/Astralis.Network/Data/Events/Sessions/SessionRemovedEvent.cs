
using Astralis.Core.Interfaces.Events;

namespace Astralis.Network.Data.Events.Sessions;

public class SessionRemovedEvent : IAstralisEvent
{
    public string SessionId { get; }

    public SessionRemovedEvent(string sessionId)
    {
        SessionId = sessionId;
    }
}
