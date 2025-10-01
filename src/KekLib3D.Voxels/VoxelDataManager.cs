using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KekLib3D.Voxels;

public class VoxelDataManager
{
    private readonly Dictionary<ushort, VoxelDefinition> _voxelDefinitions = [];

    public void LoadFromXml(ContentManager content, string filePath)
    {
        string fullPath = Path.Combine(content.RootDirectory, filePath);
        XDocument doc = XDocument.Load(fullPath);

        var root = doc.Root;
        var voxels = root.Elements("Voxel");

        foreach (var voxelElement in voxels)
        {
            ushort id = ushort.Parse(voxelElement.Attribute("id").Value);
            string name = voxelElement.Attribute("name").Value;

            var faceTextures = new Dictionary<Vector3?, string>();

            foreach (var textureElement in voxelElement.Elements("Texture"))
            {
                string side = textureElement.Attribute("side").Value.ToLower();
                string textureName = textureElement.Attribute("name").Value;

                switch (side)
                {
                    case "all": faceTextures[null] = textureName; break;
                    case "top": faceTextures[Vector3.Up] = textureName; break;
                    case "bottom": faceTextures[Vector3.Down] = textureName; break;
                    case "left": faceTextures[Vector3.Left] = textureName; break;
                    case "right": faceTextures[Vector3.Right] = textureName; break;
                    case "front": faceTextures[Vector3.Forward] = textureName; break;
                    case "back": faceTextures[Vector3.Backward] = textureName; break;
                }
            }

            _voxelDefinitions[id] = new VoxelDefinition(id, name, faceTextures);
        }
    }

    public VoxelDefinition GetVoxelDefinition(ushort id)
    {
        _voxelDefinitions.TryGetValue(id, out var definition);
        return definition;
    }

    public List<string> GetAllUniqueTextureNames() => [.. _voxelDefinitions.Values.SelectMany(def => def.GetAllTextureNames()).Distinct()];
}
