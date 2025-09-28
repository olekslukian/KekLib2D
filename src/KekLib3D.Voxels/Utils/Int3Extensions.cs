using Microsoft.Xna.Framework;

namespace KekLib3D.Voxels.Utils;

public static class Int3Extensions
{
  public static Vector3 ToVector3(this Int3 int3) => new(int3.X, int3.Y, int3.Z);
}
