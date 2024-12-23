using Astralis.Core.Interfaces.Events;
using LiteNetLib;

namespace Astralis.Network.Data.Events.Clients;

public class ClientConnectedEvent : IAstralisEvent
{
    public string SessionId { get; set; }
    public NetPeer Peer { get; set; }

    public ClientConnectedEvent(string sessionId, NetPeer peer)
    {
        SessionId = sessionId;
        Peer = peer;
    }

    public ClientConnectedEvent()
    {

    }

    public override string ToString()
    {
        return $"ClientConnectedEvent: {SessionId}";
    }


}


