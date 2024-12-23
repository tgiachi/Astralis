using System;
using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PlayerStartPositionRequestMessage : INetworkMessage
{
    public long TimeStamp { get; set; }

    public PlayerStartPositionRequestMessage()
    {
        TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
