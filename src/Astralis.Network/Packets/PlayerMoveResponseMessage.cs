using Astralis.Network.Interfaces.Messages;
using Astralis.Network.Serialization.Numerics;
using ProtoBuf;

namespace Astralis.Network.Packets;

[ProtoContract]
public class PlayerMoveResponseMessage : INetworkMessage
{
    [ProtoMember(1)] public string SessionId { get; set; }

    [ProtoMember(2)] public SerializableVector3 Position { get; set; }

    [ProtoMember(3)] public SerializableVector3 Rotation { get; set; }

    public PlayerMoveResponseMessage()
    {
    }

    public PlayerMoveResponseMessage(string sessionId, SerializableVector3 position, SerializableVector3 rotation)
    {
        SessionId = sessionId;
        Position = position;
        Rotation = rotation;
    }

    public override string ToString()
    {
        return $"PlayerMoveResponseMessage: {SessionId} Position: {Position} Rotation: {Rotation}";
    }
}
