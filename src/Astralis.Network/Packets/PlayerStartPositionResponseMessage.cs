using System.Numerics;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Serialization.Numerics;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PlayerStartPositionResponseMessage : INetworkMessage
{
    [ProtoMember(1)]
    public SerializableVector3 Position { get; set; }


    public PlayerStartPositionResponseMessage()
    {

    }


    public PlayerStartPositionResponseMessage(SerializableVector3 position)
    {
        Position = position;
    }

    public PlayerStartPositionResponseMessage(Vector3 position)
    {
        Position = new SerializableVector3(position);
    }
}
