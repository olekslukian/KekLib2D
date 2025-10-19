using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using KekLib3D.Voxels;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;

namespace Sandbox.Map;

public class VoxelMapSerializer
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public class MapData
    {
        public MapSize MapSize { get; set; }
        public List<VoxelData> Voxels { get; set; }
    }

    public class MapSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Position3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Int3 ToInt3() => new((int)X, (int)Y, (int)Z);
    }

    public class VoxelTexture
    {
        public string Default { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
    }

    public class VoxelData
    {
        public Position3D Position { get; set; }
        public string Id { get; set; }
        public VoxelTexture Texture { get; set; }


    }

    public static void SaveMap(string filePath, VoxelMap map, VoxelDataManager voxelDataManager, Vector2 mapSize)
    {
        var voxels = map.Voxels;
        var voxelDataList = new List<VoxelData>();

        foreach (var (pos, id) in voxels)
        {
            var voxelDef = voxelDataManager.GetVoxelDefinition(id);
            var texture = new VoxelTexture();

            if (voxelDef.DefaultTexture != null)
            {
                texture.Default = voxelDef.DefaultTexture;
            }

            if (voxelDef.FaceTextures != null)
            {
                foreach (var (face, textureName) in voxelDef.FaceTextures)
                {
                    if (face == Vector3.Up)
                    {
                        texture.Top = textureName;
                    }
                    else if (face == Vector3.Down)
                    {
                        texture.Bottom = textureName;
                    }
                    else if (face == Vector3.Left)
                    {
                        texture.Left = textureName;
                    }
                    else if (face == Vector3.Right)
                    {
                        texture.Right = textureName;
                    }
                    else if (face == Vector3.Forward)
                    {
                        texture.Front = textureName;
                    }
                    else if (face == Vector3.Backward)
                    {
                        texture.Back = textureName;
                    }
                }
            }

            var voxelData = new VoxelData
            {
                Position = new Position3D
                {
                    X = pos.X,
                    Y = pos.Y,
                    Z = pos.Z
                },
                Id = voxelDef.Name,
                Texture = texture
            };

            voxelDataList.Add(voxelData);

        }

        var mapData = new MapData
        {
            MapSize = new MapSize
            {
                Width = (int)mapSize.X,
                Height = (int)mapSize.Y
            },
            Voxels = voxelDataList
        };


        var json = JsonSerializer.Serialize(mapData, _serializerOptions);

        File.WriteAllText(filePath, json);
    }

    public static MapData LoadMap(string filePath)

    {
        var json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<MapData>(json);
    }
}


