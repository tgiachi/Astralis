using System;
using Astralis.Network.Interfaces.Messages;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class MotdRequestMessage : INetworkMessage
{
    [ProtoMember(1)]
    public long TimeStamp { get; set; }

    public MotdRequestMessage()
    {
        TimeStamp = DateTime.Now.Ticks;
    }

}
