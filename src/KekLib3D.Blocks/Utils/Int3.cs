using System;

namespace KekLib3D.Voxels.Utils;

public struct Int3(int x, int y, int z) : IEquatable<Int3>
{
    public int X = x;
    public int Y = y;
    public int Z = z;

    public readonly bool Equals(Int3 other) => X == other.X && Y == other.Y && Z == other.Z;
    public override readonly bool Equals(object obj) => obj is Int3 other && Equals(other);

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static Int3 operator +(Int3 a, Int3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public override readonly string ToString() => $"({X}, {Y}, {Z})";

    public static bool operator ==(Int3 left, Int3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Int3 left, Int3 right)
    {
        return !(left == right);
    }
}
