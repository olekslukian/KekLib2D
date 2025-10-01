
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KekLib3D.Voxels;

public class VoxelDefinition
{
    public ushort Id { get; private set; }
    public string Name { get; private set; }
    public Dictionary<Vector3?, string> _faceTextures;
    private readonly string _defaultTexture;

    public VoxelDefinition(ushort id, string name, string defaultTexture, Dictionary<Vector3?, string> faceTextures)
    {
        Id = id;
        Name = name;
        _defaultTexture = defaultTexture;
        _faceTextures = faceTextures ?? [];
    }

    public string GetTextureNameForFace(Vector3 face)
    {
        if (_faceTextures.TryGetValue(face, out var texture))
        {
            return texture;
        }

        return _defaultTexture ?? "error_texture";
    }

    public IEnumerable<string> GetAllTextureNames()
    {
        var textures = new HashSet<string>(_faceTextures.Values);

        if (!string.IsNullOrEmpty(_defaultTexture))
        {
            textures.Add(_defaultTexture);
        }

        return textures;
    }
}
