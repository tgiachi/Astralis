using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class VersionRequestMessage : INetworkMessage
{
    public override string ToString()
    {
        return "VersionRequestMessage";
    }
}
