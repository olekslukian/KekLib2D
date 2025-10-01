
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KekLib3D.Voxels;

public class VoxelDefinition(ushort id, string name, Dictionary<Vector3?, string> faceTextures)
{
    public ushort Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public Dictionary<Vector3?, string> _faceTextures = faceTextures;

    public string GetTextureNameForFace(Vector3 face)
    {
        if (_faceTextures.TryGetValue(face, out var texture))
            return texture;

        return _faceTextures.GetValueOrDefault(null, "error_texture");
    }

    public IEnumerable<string> GetAllTextureNames()
    {
        var textures = new HashSet<string>(_faceTextures.Values);

        return textures;
    }
}
