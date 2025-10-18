
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Base;

public interface IGameObject2D : IGameObject
{
    Vector2 Position { get; set; }
    Vector2 Rotation { get; set; }
    Vector2 Scale { get; set; }
}
