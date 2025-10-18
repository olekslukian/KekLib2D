using KekLib2D.Core.Base;
using Microsoft.Xna.Framework;

namespace KekLib3D.Base;

public interface IGameObject3D : IGameObject
{
    Vector3 Position { get; set; }
    Vector3 Rotation { get; set; }
    Vector3 Scale { get; set; }
}
