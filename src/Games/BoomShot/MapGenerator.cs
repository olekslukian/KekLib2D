using System;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Voxels.Utils;

namespace BoomShot;

public class MapGenerator(string[] terrainIds, string[] structureIds)
{
    private readonly string[] _terrainIds = terrainIds;
    private readonly string[] _structureIds = structureIds;

    public VoxelMap GenerateMap(int width, int depth, int? seed)
    {
        var map = new VoxelMap();
        var random = new Random(seed ?? Environment.TickCount);

        for (int ix = 0; ix < width; ix++)
            for (int iz = 0; iz < depth; iz++)
            {
                int x = ix - width / 2;
                int z = iz - depth / 2;

                var terrainId = _terrainIds[0];
                map.Set(new Int3(x, 0, z), terrainId);

                if (random.NextDouble() < 0.2)
                {
                    string id = _structureIds[random.Next(_structureIds.Length)];
                    int height = random.Next(1, 4);

                    for (int y = 1; y <= height; y++)
                    {
                        map.Set(new Int3(x, y, z), id);
                    }
                }
            }

        return map;
    }

}
