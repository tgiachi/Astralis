using System.Numerics;
using ProtoBuf;

namespace Astralis.Network.Serialization.Numerics;

[ProtoContract]
public class SerializableVector3
{

    [ProtoMember(1)]
    public float X { get; set; }


    [ProtoMember(2)]
    public float Y { get; set; }


    [ProtoMember(3)]
    public float Z { get; set; }


    public SerializableVector3()
    {
    }

    public SerializableVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public SerializableVector3(Vector3 vector3)
    {
        X = vector3.X;
        Y = vector3.Y;
        Z = vector3.Z;
    }


    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public override string ToString()
    {
        return $"SerializableVector3: {X}, {Y}, {Z}";
    }
}
