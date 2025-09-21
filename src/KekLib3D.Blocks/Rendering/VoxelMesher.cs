using System;
using System.Collections.Generic;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public static class VoxelMesher
{
    readonly struct FaceDef(Int3 n, Vector3[] v)
    {
        public readonly Int3 N = n;
        public readonly Vector3[] V = v;
    }

    static readonly FaceDef[] Faces = [
        new(new Int3(1, 0, 0), [new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f)]),
        new(new Int3(-1, 0, 0), [new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, 0.5f)]),
        new(new Int3(0, 1, 0), [new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f)]),
        new(new Int3(0, -1, 0), [new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f)]),
        new(new Int3(0, 0, 1), [new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)]),
        new(new Int3(0, 0, -1), [new Vector3(0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)])
    ];

    public static void Build(VoxelMap map, out VertexPositionColor[] vertices, out short[] indices)
    {
        var v = new List<VertexPositionColor>(map.Voxels.Count * 24);
        var i = new List<short>(map.Voxels.Count * 36);
        short baseIndex = 0;

        foreach (var kv in map.Voxels)
        {
            var p = kv.Key;
            Vector3 center = new(p.X + 0.5f, p.Y + 0.5f, p.Z + 0.5f);

            foreach (var f in Faces)
            {
                Int3 neighbor = new(p.X + f.N.X, p.Y + f.N.Y, p.Z + f.N.Z);
                if (map.Has(neighbor)) continue;

                // Default color, will change, when i will add textures
                var color = Color.White;

                v.Add(new VertexPositionColor(center + f.V[0], color));
                v.Add(new VertexPositionColor(center + f.V[1], color));
                v.Add(new VertexPositionColor(center + f.V[2], color));
                v.Add(new VertexPositionColor(center + f.V[3], color));

                i.Add((short)(baseIndex + 0));
                i.Add((short)(baseIndex + 1));
                i.Add((short)(baseIndex + 2));
                i.Add((short)(baseIndex + 0));
                i.Add((short)(baseIndex + 2));
                i.Add((short)(baseIndex + 3));

                baseIndex += 4;
            }
        }

        vertices = [.. v];
        indices = [.. i];
    }
}
