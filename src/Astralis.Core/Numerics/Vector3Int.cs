using System;

namespace Astralis.Core.Numerics;

public readonly struct Vector3Int(int x, int y, int z) : IEquatable<Vector3Int>
{
    public readonly int X = x;
    public readonly int Y = y;
    public readonly int Z = z;

    public static readonly Vector3Int Zero = new(0, 0, 0);
    public static readonly Vector3Int One = new(1, 1, 1);
    public static readonly Vector3Int Up = new(0, 1, 0);
    public static readonly Vector3Int Down = new(0, -1, 0);
    public static readonly Vector3Int Right = new(1, 0, 0);
    public static readonly Vector3Int Left = new(-1, 0, 0);
    public static readonly Vector3Int Forward = new(0, 0, 1);
    public static readonly Vector3Int Back = new(0, 0, -1);

    public static Vector3Int operator +(Vector3Int a, Vector3Int b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3Int operator -(Vector3Int a, Vector3Int b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3Int operator *(Vector3Int a, int scalar) =>
        new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public static Vector3Int operator *(int scalar, Vector3Int a) => a * scalar;

    public static Vector3Int operator /(Vector3Int a, int scalar) =>
        new(a.X / scalar, a.Y / scalar, a.Z / scalar);

    public static Vector3Int operator -(Vector3Int a) =>
        new(-a.X, -a.Y, -a.Z);

    public static bool operator ==(Vector3Int lhs, Vector3Int rhs) =>
        lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;

    public static bool operator !=(Vector3Int lhs, Vector3Int rhs) => !(lhs == rhs);

    public static Vector3Int operator *(Vector3Int a, Vector3Int b) =>
        new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

    public static Vector3Int operator /(Vector3Int a, Vector3Int b) =>
        new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    public static Vector3Int operator >> (Vector3Int a, int shift) =>
        new(a.X >> shift, a.Y >> shift, a.Z >> shift);

    public static Vector3Int operator <<(Vector3Int a, int shift) =>
        new(a.X << shift, a.Y << shift, a.Z << shift);

    public static Vector3Int operator &(Vector3Int a, int mask) =>
        new(a.X & mask, a.Y & mask, a.Z & mask);

    public static Vector3Int operator |(Vector3Int a, int mask) =>
        new(a.X | mask, a.Y | mask, a.Z | mask);

    public static Vector3Int operator ^(Vector3Int a, int mask) => new(a.X ^ mask, a.Y ^ mask, a.Z ^ mask);

    public static Vector3Int Min(Vector3Int a, Vector3Int b) =>
        new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

    public static Vector3Int Max(Vector3Int a, Vector3Int b) =>
        new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    public static Vector3Int Clamp(Vector3Int value, Vector3Int min, Vector3Int max) =>
        Max(Min(value, max), min);

    public int ManhattanDistance(Vector3Int other) =>
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

    public float Distance(Vector3Int other)
    {
        Vector3Int diff = this - other;
        return MathF.Sqrt(diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z);
    }

    public static float Distance(Vector3Int a, Vector3Int b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float dz = a.Z - b.Z;

        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static float DistanceSquared(Vector3Int a, Vector3Int b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float dz = a.Z - b.Z;

        return dx * dx + dy * dy + dz * dz;
    }

    public int SqrMagnitude => X * X + Y * Y + Z * Z;

    public float Magnitude => MathF.Sqrt(SqrMagnitude);

    public Vector3Int Normalize()
    {
        float magnitude = Magnitude;
        return new Vector3Int((int)(X / magnitude), (int)(Y / magnitude), (int)(Z / magnitude));
    }

    public Vector3Int Abs()
    {
        return new Vector3Int(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public string ToString(string format) =>
        $"({X.ToString(format)}, {Y.ToString(format)}, {Z.ToString(format)})";

    public static explicit operator System.Numerics.Vector3(Vector3Int v) =>
        new(v.X, v.Y, v.Z);

    public static explicit operator Vector3Int(System.Numerics.Vector3 v) =>
        new((int)v.X, (int)v.Y, (int)v.Z);

    public bool Equals(Vector3Int other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    public override bool Equals(object? obj) =>
        obj is Vector3Int other && Equals(other);

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static Vector3Int FloorToInt(System.Numerics.Vector3 v) =>
        new((int)MathF.Floor(v.X), (int)MathF.Floor(v.Y), (int)MathF.Floor(v.Z));

    public static Vector3Int CeilToInt(System.Numerics.Vector3 v) =>
        new((int)MathF.Ceiling(v.X), (int)MathF.Ceiling(v.Y), (int)MathF.Ceiling(v.Z));

    public static Vector3Int RoundToInt(System.Numerics.Vector3 v) =>
        new((int)MathF.Round(v.X), (int)MathF.Round(v.Y), (int)MathF.Round(v.Z));

    // Metodi per il chunk
    public Vector3Int GetChunkPosition(int chunkSize)
    {
        return new Vector3Int(
            X >= 0 ? X / chunkSize : (X - chunkSize + 1) / chunkSize,
            Y >= 0 ? Y / chunkSize : (Y - chunkSize + 1) / chunkSize,
            Z >= 0 ? Z / chunkSize : (Z - chunkSize + 1) / chunkSize
        );
    }

    public Vector3Int GetLocalPosition(int chunkSize)
    {
        return new Vector3Int(
            X >= 0 ? X % chunkSize : (chunkSize - 1) + ((X + 1) % chunkSize),
            Y >= 0 ? Y % chunkSize : (chunkSize - 1) + ((Y + 1) % chunkSize),
            Z >= 0 ? Z % chunkSize : (chunkSize - 1) + ((Z + 1) % chunkSize)
        );
    }
}
