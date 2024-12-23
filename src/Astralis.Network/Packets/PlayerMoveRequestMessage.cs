using System.Numerics;
using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Serialization.Numerics;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PlayerMoveRequestMessage : INetworkMessage
{
    [ProtoMember(1)]
    public SerializableVector3 Position { get; set; }

    [ProtoMember(2)]
    public SerializableVector3 Rotation { get; set; }


    public PlayerMoveRequestMessage()
    {

    }

    public PlayerMoveRequestMessage(SerializableVector3 position, SerializableVector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public PlayerMoveRequestMessage(Vector3 position, Vector3 rotation)
    {
        Position = new SerializableVector3(position);
        Rotation = new SerializableVector3(rotation);
    }

    public override string ToString()
    {
        return $"PlayerMoveRequestMessage: {Position} Rotation: {Rotation}";
    }
}
