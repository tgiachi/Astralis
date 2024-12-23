using Astralis.Core.Interfaces.Events;
using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Data.Events.Network;

public class SendMessageEvent : IAstralisEvent
{
    public string SessionId { get; set; }

    public INetworkMessage Message { get; set; }

    public SendMessageEvent(string sessionId, INetworkMessage message)
    {
        SessionId = sessionId;
        Message = message;
    }


    public override string ToString()
    {
        return $"SessionId: {(SessionId == string.Empty ? "Broadcast" : SessionId)}, Message: {Message.GetType().Name}";
    }
}
