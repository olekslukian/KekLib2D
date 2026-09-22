using KekLib3D.Voxels.Utils;

namespace KekLib3D.Voxels.Serialization;

public struct Position3D
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public readonly Int3 ToInt3() => new((int)X, (int)Y, (int)Z);
}