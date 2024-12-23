using Astralis.Core.Numerics;
using ProtoBuf;

namespace Astralis.Network.Serialization.Numerics;

[ProtoContract]
public class SerializableVector3Int
{
    [ProtoMember(1)] public int X { get; set; }

    [ProtoMember(2)] public int Y { get; set; }

    [ProtoMember(3)] public int Z { get; set; }


    public SerializableVector3Int()
    {
    }


    public SerializableVector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public SerializableVector3Int(Vector3Int vector3Int)
    {
        X = vector3Int.X;
        Y = vector3Int.Y;
        Z = vector3Int.Z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(X, Y, Z);
    }

    public override string ToString()
    {
        return $"SerializableVector3Int: {X}, {Y}, {Z}";
    }
}
