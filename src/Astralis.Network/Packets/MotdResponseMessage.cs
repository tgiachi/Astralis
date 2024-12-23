using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class MotdResponseMessage : INetworkMessage
{

    [ProtoMember(1)]
    public string Message { get; set; }

    public MotdResponseMessage()
    {

    }

    public MotdResponseMessage(string message)
    {
        Message = message;
    }
}
