using System;
using System.Numerics;
using Astralis.Core.Numerics;

namespace Astralis.Core.Extensions;

public static class MathExtension
{
    public static float SqrMagnitude(this Vector3 vector)
    {
        return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }


    public static float SqrMagnitude(this Vector3 pointA, Vector3 pointB)
    {
        var difference = pointA - pointB;
        return SqrMagnitude(difference);
    }

    public static int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }

    public static float RandomFloat(float min, float max) => (float)new Random().NextDouble() * (max - min) + min;

    public static Vector3 RandomVector3(this Vector3 vector, float min, float max)
    {
        var rnd = new Random();

        return new Vector3(RandomFloat(min, max), RandomFloat(min, max), RandomFloat(min, max));
    }

    public static Vector3Int RandomVector3Int(this Vector3Int vector, int min, int max)
    {
        var rnd = new Random();

        return new Vector3Int(rnd.Next(min, max), rnd.Next(min, max), rnd.Next(min, max));
    }

    public static Vector3 ToVector3(this Vector3Int vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
}
