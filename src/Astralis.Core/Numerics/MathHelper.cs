using System;
using System.Numerics;
using Astralis.Core.Types;

namespace Astralis.Core.Numerics;

public static class MathHelper
{
    public static float DegreesToRadians(float degrees)
    {
        return MathF.PI / 180f * degrees;
    }

    public static void EncodeVector3Int(Span<byte> vector, int a, int b, int c)
    {
        var aBytes = BitConverter.GetBytes(a);
        var bBytes = BitConverter.GetBytes(b);
        var cBytes = BitConverter.GetBytes(c);
        aBytes.CopyTo(vector);
        bBytes.CopyTo(vector[4..]);
        cBytes.CopyTo(vector[8..]);
    }
    /*public static Vector3D<int> DecodeVector3Int(ReadOnlySpan<byte> bytes) {
        return new(BitConverter.ToInt32(bytes), BitConverter.ToInt32(bytes[4..]), BitConverter.ToInt32(bytes[8..]));
    }*/

    public static Vector3Direction GetDirection(Vector3 previous, Vector3 current)
    {
        Vector3 delta = current - previous;
        Vector3 direction = Vector3.Normalize(delta);

        if (Math.Abs(direction.Z) > Math.Abs(direction.X) && Math.Abs(direction.Z) > Math.Abs(direction.Y))
        {
            return direction.Z > 0 ? Vector3Direction.North : Vector3Direction.South;
        }

        if (Math.Abs(direction.X) > Math.Abs(direction.Z) && Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            return direction.X > 0 ? Vector3Direction.East : Vector3Direction.West;
        }

        if (Math.Abs(direction.Y) > Math.Abs(direction.Z) && Math.Abs(direction.Y) > Math.Abs(direction.X))
        {
            return direction.Y > 0 ? Vector3Direction.Up : Vector3Direction.Down;
        }

        return Vector3Direction.None;
    }
}
