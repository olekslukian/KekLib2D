using System.Collections.Generic;
using KekLib3D.Voxels.Serialization;
using KekLib3D.Voxels.Utils;

namespace KekLib3D.Voxels.Rendering;

public class VoxelMap
{
    readonly Dictionary<Int3, string> _voxels = [];
    public IReadOnlyDictionary<Int3, string> Voxels => _voxels;
    public bool IsDirty { get; private set; } = true;

    public bool Has(Int3 pos) => _voxels.ContainsKey(pos);

    public void FromMapData(MapData mapData)
    {
        _voxels.Clear();

        foreach (var voxel in mapData.Voxels)
        {
            _voxels[voxel.Position.ToInt3()] = voxel.Id;
        }

        IsDirty = true;
    }

    public void Set(Int3 pos, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Remove(pos);
            return;
        }

        if (!_voxels.TryGetValue(pos, out var existing) || existing != id)
        {
            _voxels[pos] = id;
            IsDirty = true;
        }
    }

    public void Remove(Int3 pos)
    {
        if (_voxels.Remove(pos)) IsDirty = true;
    }

    public void ClearDirty() => IsDirty = false;
}
