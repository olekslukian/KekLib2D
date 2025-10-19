
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KekLib3D.Voxels;

public class VoxelDefinition(ushort id, string name, string defaultTexture, Dictionary<Vector3?, string> faceTextures)
{
    public ushort Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public Dictionary<Vector3?, string> FaceTextures { get; private set; } = faceTextures ?? [];
    public string DefaultTexture { get; private set; } = defaultTexture;

    public string GetTextureNameForFace(Vector3 face)
    {
        if (FaceTextures.TryGetValue(face, out var texture))
        {
            return texture;
        }

        return DefaultTexture ?? "error_texture";
    }

    public IEnumerable<string> GetAllTextureNames()
    {
        var textures = new HashSet<string>(FaceTextures.Values);

        if (!string.IsNullOrEmpty(DefaultTexture))
        {
            textures.Add(DefaultTexture);
        }

        return textures;
    }
}
