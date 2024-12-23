using System;
using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PongResponseMessage : INetworkMessage
{
    [ProtoMember(1)] public long Timestamp { get; set; }

    public PongResponseMessage()
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return $"PongResponseMessage: {Timestamp}";
    }
}
