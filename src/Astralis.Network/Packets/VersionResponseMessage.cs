using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class VersionResponseMessage : INetworkMessage
{
    [ProtoMember(1)] public string Version { get; set; }

    public VersionResponseMessage()
    {
    }

    public VersionResponseMessage(string version)
    {
        Version = version;
    }

    public override string ToString()
    {
        return $"VersionResponseMessage: {Version}";
    }
}
