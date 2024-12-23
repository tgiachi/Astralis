using System;

namespace Astralis.Core.Numerics;

public readonly struct Vector2Int(int x, int y) : IEquatable<Vector2Int>
{
    public readonly int X = x;
    public readonly int Y = y;

    public static readonly Vector2Int Zero = new(0, 0);
    public static readonly Vector2Int One = new(1, 1);
    public static readonly Vector2Int Up = new(0, 1);
    public static readonly Vector2Int Down = new(0, -1);
    public static readonly Vector2Int Right = new(1, 0);
    public static readonly Vector2Int Left = new(-1, 0);

    public static Vector2Int operator +(Vector2Int a, Vector2Int b) =>
        new(a.X + b.X, a.Y + b.Y);

    public static Vector2Int operator -(Vector2Int a, Vector2Int b) =>
        new(a.X - b.X, a.Y - b.Y);

    public static Vector2Int operator *(Vector2Int a, int scalar) =>
        new(a.X * scalar, a.Y * scalar);

    public static Vector2Int operator *(int scalar, Vector2Int a) => a * scalar;

    public static Vector2Int operator /(Vector2Int a, int scalar) =>
        new(a.X / scalar, a.Y / scalar);

    public static Vector2Int operator -(Vector2Int a) =>
        new(-a.X, -a.Y);

    public static bool operator ==(Vector2Int lhs, Vector2Int rhs) =>
        lhs.X == rhs.X && lhs.Y == rhs.Y;

    public static bool operator !=(Vector2Int lhs, Vector2Int rhs) => !(lhs == rhs);

    public static Vector2Int Min(Vector2Int a, Vector2Int b) =>
        new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

    public static Vector2Int Max(Vector2Int a, Vector2Int b) =>
        new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    public static Vector2Int Clamp(Vector2Int value, Vector2Int min, Vector2Int max) =>
        Max(Min(value, max), min);

    public int ManhattanDistance(Vector2Int other) =>
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

    public float Distance(Vector2Int other)
    {
        Vector2Int diff = this - other;
        return MathF.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
    }

    public static float Distance(Vector2Int a, Vector2Int b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;

        return MathF.Sqrt(dx * dx + dy * dy);
    }

    public static float DistanceSquared(Vector2Int a, Vector2Int b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;

        return dx * dx + dy * dy;
    }

    public int SqrMagnitude => X * X + Y * Y;

    public float Magnitude => MathF.Sqrt(SqrMagnitude);

    public Vector2Int Normalize()
    {
        float magnitude = Magnitude;
        return new Vector2Int((int)(X / magnitude), (int)(Y / magnitude));
    }

    public Vector2Int Abs()
    {
        return new Vector2Int(Math.Abs(X), Math.Abs(Y));
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public string ToString(string format) =>
        $"({X.ToString(format)}, {Y.ToString(format)})";

    public static explicit operator System.Numerics.Vector2(Vector2Int v) =>
        new(v.X, v.Y);

    public static explicit operator Vector2Int(System.Numerics.Vector2 v) =>
        new((int)v.X, (int)v.Y);

    public bool Equals(Vector2Int other) =>
        X == other.X && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is Vector2Int other && Equals(other);

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static Vector2Int FloorToInt(System.Numerics.Vector2 v) =>
        new((int)MathF.Floor(v.X), (int)MathF.Floor(v.Y));

    public static Vector2Int CeilToInt(System.Numerics.Vector2 v) =>
        new((int)MathF.Ceiling(v.X), (int)MathF.Ceiling(v.Y));

    public static Vector2Int RoundToInt(System.Numerics.Vector2 v) =>
        new((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
}
