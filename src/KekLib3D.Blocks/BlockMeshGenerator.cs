
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Blocks;

public static class BlockMeshGenerator
{
    public static (VertexPositionColor[], short[]) CreateCube(Color color)
    {
        Vector3 p1 = new(-0.5f, 0.5f, 0.5f);
        Vector3 p2 = new(0.5f, 0.5f, 0.5f);
        Vector3 p3 = new(0.5f, -0.5f, 0.5f);
        Vector3 p4 = new(-0.5f, -0.5f, 0.5f);
        Vector3 p5 = new(-0.5f, 0.5f, -0.5f);
        Vector3 p6 = new(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new(0.5f, -0.5f, -0.5f);
        Vector3 p8 = new(-0.5f, -0.5f, -0.5f);

        VertexPositionColor[] vertices =
        [
            // Front face
        new VertexPositionColor(p1, color), new VertexPositionColor(p2, color), new VertexPositionColor(p3, color), new VertexPositionColor(p4, color),
        // Back face
        new VertexPositionColor(p6, color), new VertexPositionColor(p5, color), new VertexPositionColor(p8, color), new VertexPositionColor(p7, color),
        // Top face
        new VertexPositionColor(p5, color), new VertexPositionColor(p6, color), new VertexPositionColor(p2, color), new VertexPositionColor(p1, color),
        // Bottom face
        new VertexPositionColor(p4, color), new VertexPositionColor(p3, color), new VertexPositionColor(p7, color), new VertexPositionColor(p8, color),
        // Right face
        new VertexPositionColor(p2, color), new VertexPositionColor(p6, color), new VertexPositionColor(p7, color), new VertexPositionColor(p3, color),
        // Left face
        new VertexPositionColor(p5, color), new VertexPositionColor(p1, color), new VertexPositionColor(p4, color), new VertexPositionColor(p8, color)
        ];

        short[] indices =
        [
            // Front face
        0, 1, 2, 0, 2, 3,
        // Back face
        4, 5, 6, 4, 6, 7,
        // Top face
        8, 9, 10, 8, 10, 11,
        // Bottom face
        12, 13, 14, 12, 14, 15,
        // Right face
        16, 17, 18, 16, 18, 19,
        // Left face
        20, 21, 22, 20, 22, 23
        ];

        return (vertices, indices);
    }
}
