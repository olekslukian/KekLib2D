using System;
using System.Collections.Generic;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public static class VoxelMesher
{
    readonly struct FaceDef(Int3 n, Vector3[] v, Vector3 normal)
    {
        public readonly Int3 N = n;
        public readonly Vector3[] V = v;
        public readonly Vector3 Normal = normal;
    }

    static readonly FaceDef[] Faces = [
         // Front face (+Z)
         new(new Int3(0, 0, 1), [
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f)
        ], Vector3.Forward),

        // Back face (-Z)
        new(new Int3(0, 0, -1), [
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        ], Vector3.Backward),

        // Left face (-X)
        new(new Int3(-1, 0, 0), [
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
        ], Vector3.Left),

        // Right face (+X)
        new(new Int3(1, 0, 0), [
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        ], Vector3.Right),

        // Top face (+Y)
        new(new Int3(0, 1, 0), [
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f)
        ], Vector3.Up),

        // Bottom face (-Y)
        new(new Int3(0, -1, 0), [
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        ], Vector3.Down)
     ];

    public static void Build(VoxelMap map, out VertexPositionColorNormal[] vertices, out short[] indices)
    {
        var v = new List<VertexPositionColorNormal>();
        var i = new List<short>();
        short baseIndex = 0;

        foreach (var kv in map.Voxels)
        {
            var p = kv.Key;
            Vector3 center = new(p.X + 0.5f, p.Y + 0.5f, p.Z + 0.5f);

            foreach (var face in Faces)
            {
                Int3 neighbor = new(p.X + face.N.X, p.Y + face.N.Y, p.Z + face.N.Z);
                if (map.Has(neighbor)) continue;

                var color = GetBlockColor(kv.Value);

                foreach (var vertex in face.V)
                {
                    v.Add(new VertexPositionColorNormal(center + vertex, color, face.Normal));
                }

                i.AddRange([
                   (short)(baseIndex + 0), (short)(baseIndex + 1), (short)(baseIndex + 2),
                    (short)(baseIndex + 0), (short)(baseIndex + 2), (short)(baseIndex + 3)
               ]);

                baseIndex += 4;
            }
        }

        vertices = [.. v];
        indices = [.. i];
    }

    private static Color GetBlockColor(ushort blockId)
    {
        return blockId switch
        {
            1 => Color.White,
            2 => Color.Red,
            3 => Color.Blue,
            _ => Color.Gray
        };
    }
}
