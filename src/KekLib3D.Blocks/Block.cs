
using Microsoft.Xna.Framework;

namespace KekLib3D.Blocks;

public readonly struct Block(Vector3 position, Color color)
{
    public Vector3 Position { get; } = position;
    public Color Color { get; } = color;
}
