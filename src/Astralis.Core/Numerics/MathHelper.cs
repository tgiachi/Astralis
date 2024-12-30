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

    public static Vector3 FromQuaternion(Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (Math.Abs(sinp) >= 1)
        {
            angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
        }
        else
        {
            angles.Y = (float)Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }


    public static Quaternion ToQuaternion(Vector3 v)
    {

        float cy = (float)Math.Cos(v.Z * 0.5);
        float sy = (float)Math.Sin(v.Z * 0.5);
        float cp = (float)Math.Cos(v.Y * 0.5);
        float sp = (float)Math.Sin(v.Y * 0.5);
        float cr = (float)Math.Cos(v.X * 0.5);
        float sr = (float)Math.Sin(v.X * 0.5);

        return new Quaternion
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };

    }
}
