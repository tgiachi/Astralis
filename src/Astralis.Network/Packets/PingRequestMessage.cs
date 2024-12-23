using System;
using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PingRequestMessage : INetworkMessage
{
    [ProtoMember(1)] public long Timestamp { get; set; }

    public PingRequestMessage()
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return $"PingRequestMessage: {Timestamp}";
    }
}
