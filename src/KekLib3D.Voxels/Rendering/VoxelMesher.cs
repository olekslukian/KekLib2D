using System.Collections.Generic;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public static class VoxelMesher
{
    readonly struct FaceDef(Int3 neighbourPos, Vector3[] vertices, Vector3 normal)
    {
        public readonly Int3 NeighbourPos = neighbourPos;
        public readonly Vector3[] Vertices = vertices;
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

    public static void Build(VoxelMap map, VoxelDataManager dataManager, VoxelTextureAtlas atlas, out VertexPositionNormalTexture[] vertices, out short[] indices)
    {
        var vertList = new List<VertexPositionNormalTexture>();
        var indexList = new List<short>();
        short baseIndex = 0;

        Vector2[] faceUVs = [
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1)
        ];

        foreach (var (pos, voxelId) in map.Voxels)
        {
            var definition = dataManager.GetVoxelDefinition(voxelId);
            if (definition == null) continue;

            Vector3 center = new(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f);

            foreach (var face in Faces)
            {
                if (map.Has(pos + face.NeighbourPos)) continue;

                string textureName = definition.GetTextureNameForFace(face.Normal);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 finalUv = atlas.GetAtlasUv(textureName, faceUVs[i]);

                    vertList.Add(new VertexPositionNormalTexture(
                        position: center + face.Vertices[i],
                        normal: face.Normal,
                        textureCoordinate: finalUv
                    ));
                }

                indexList.AddRange([
                             baseIndex, (short)(baseIndex + 1), (short)(baseIndex + 2),
                baseIndex , (short)(baseIndex + 2), (short)(baseIndex + 3)
                         ]);

                baseIndex += 4;
            }
        }

        vertices = vertList.ToArray();
        indices = indexList.ToArray();
    }
}
