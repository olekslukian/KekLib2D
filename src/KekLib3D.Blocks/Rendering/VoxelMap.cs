using System;
using System.Collections.Generic;
using KekLib3D.Voxels.Utils;

namespace KekLib3D.Voxels.Rendering;

public class VoxelMap
{
    readonly Dictionary<Int3, ushort> _voxels = [];
    public IReadOnlyDictionary<Int3, ushort> Voxels => _voxels;
    public bool IsDirty { get; private set; }

    public bool Has(Int3 pos) => _voxels.ContainsKey(pos);
    public void Set(Int3 pos, ushort id)
    {
        if (id == 0)
        {
            if (_voxels.Remove(pos)) IsDirty = true;
            return;
        }

        if (!_voxels.TryGetValue(pos, out var ex) || ex != id)
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
