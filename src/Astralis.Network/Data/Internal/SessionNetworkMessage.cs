using Astralis.Network.Interfaces.Messages;

namespace Astralis.Network.Data.Internal;

public class SessionNetworkMessage
{
    public string SessionId { get; set; }

    public INetworkMessage Packet { get; set; }

    public SessionNetworkMessage(string sessionId, INetworkMessage packet)
    {
        SessionId = sessionId;
        Packet = packet;
    }

    public SessionNetworkMessage()
    {

    }

    public override string ToString() => $"SessionId: {SessionId}, Packet: {Packet.GetType()}";
}
