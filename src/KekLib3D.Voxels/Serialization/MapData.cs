
using System.Collections.Generic;

namespace KekLib3D.Voxels.Serialization;

public struct MapData
{
    public MapSize MapSize { get; set; }
    public List<VoxelData> Voxels { get; set; }
}