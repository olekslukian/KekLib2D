using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KekLib3D.Voxels;

public class VoxelDataManager
{
    private readonly Dictionary<string, Voxel> _voxels = [];

    public void LoadFromXml(ContentManager content, string filePath)
    {
        string fullPath = Path.Combine(content.RootDirectory, filePath);
        using Stream stream = TitleContainer.OpenStream(fullPath);
        using XmlReader reader = XmlReader.Create(stream);
        XDocument doc = XDocument.Load(reader);

        var root = doc.Root;
        var voxels = root.Elements("Voxel");

        foreach (var voxelElement in voxels)
        {
            string id = voxelElement.Attribute("id").Value;

            var faceTextures = new Dictionary<Vector3?, string>();

            string defaultTextureName = null;

            foreach (var textureElement in voxelElement.Elements("Texture"))
            {
                string side = textureElement.Attribute("side").Value.ToLower();
                string textureName = textureElement.Attribute("name").Value;

                if (side == "default")
                {
                    defaultTextureName = textureName;
                    continue;
                }

                switch (side)
                {
                    case "top": faceTextures[Vector3.Up] = textureName; break;
                    case "bottom": faceTextures[Vector3.Down] = textureName; break;
                    case "left": faceTextures[Vector3.Left] = textureName; break;
                    case "right": faceTextures[Vector3.Right] = textureName; break;
                    case "front": faceTextures[Vector3.Forward] = textureName; break;
                    case "back": faceTextures[Vector3.Backward] = textureName; break;
                }
            }

            _voxels[id] = new Voxel(id, defaultTextureName, faceTextures);
        }
    }

    public Voxel GetVoxelDefinition(string id)
    {
        _voxels.TryGetValue(id, out var definition);
        return definition;
    }

    public IEnumerable<string> GetVoxelIds() => _voxels.Keys;

    public List<string> GetAllUniqueTextureNames() => [.. _voxels.Values.SelectMany(def => def.GetAllTextureNames()).Distinct()];
}
