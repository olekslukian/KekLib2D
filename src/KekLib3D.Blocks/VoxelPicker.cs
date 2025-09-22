using System;
using System.Reflection.Metadata;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;

namespace KekLib3D.Voxels;

public enum HitType : byte
{
    None,
    Ground,
    Block
}

public readonly struct PickResult(HitType type, Int3 hitVoxel, Int3 placePosition, Int3 faceNormal, Vector3 groundCenter)
{
    public readonly HitType Type = type;
    public readonly Int3 HitVoxel = hitVoxel;
    public readonly Int3 PlacePosition = placePosition;
    public readonly Int3 FaceNormal = faceNormal;
    public readonly Vector3 GroundCenter = groundCenter;
}

public static class VoxelPicker
{
    public static PickResult Pick(Ray ray, VoxelMap map, float maxDist = 100f)
    {
        float? groundDist = ray.Intersects(new Plane(Vector3.Up, 0));
        float bestDist = float.MaxValue;
        bool voxelHit = false;
        Int3 hitVoxel = default;
        Int3 faceNormal = default;

        foreach (var kv in map.Voxels)
        {
            var p = kv.Key;
            Vector3 center = new(p.X + 0.5f, p.Y + 0.5f, p.Z + 0.5f);
            BoundingBox box = new(center - new Vector3(0.5f), center + new Vector3(0.5f));
            float? d = ray.Intersects(box);

            if (d.HasValue && d.Value < bestDist && d.Value <= maxDist)
            {
                bestDist = d.Value;
                voxelHit = true;
                hitVoxel = p;
                faceNormal = ApproximateFaceNormal(center, ray, d.Value);
            }
        }

        if (voxelHit)
        {
            Int3 placePos = hitVoxel + faceNormal;

            return new PickResult(HitType.Block, hitVoxel, placePos, faceNormal, Vector3.Zero);
        }

        if (groundDist.HasValue && groundDist.Value <= maxDist)
        {
            Vector3 gHit = ray.Position + ray.Direction * groundDist.Value;
            int gx = (int)MathF.Floor(gHit.X);
            int gz = (int)MathF.Floor(gHit.Z);
            Int3 place = new(gx, 0, gz);

            return new PickResult(HitType.Ground, default, place, new Int3(0, 1, 0), new Vector3(gx + 0.5f, 0f, gz + 0.5f));
        }

        return new PickResult(HitType.None, default, default, default, Vector3.Zero);
    }

    static Int3 ApproximateFaceNormal(Vector3 center, Ray ray, float dist)
    {
        Vector3 hit = ray.Position + ray.Direction * dist;
        Vector3 local = hit - center;

        float ax = MathF.Abs(local.X);
        float ay = MathF.Abs(local.Y);
        float az = MathF.Abs(local.Z);

        if (ax > ay && ax > az) return new Int3(local.X > 0 ? 1 : -1, 0, 0);
        if (ay > ax && ay > az) return new Int3(0, local.Y > 0 ? 1 : -1, 0);

        return new Int3(0, 0, local.Z > 0 ? 1 : -1);
    }
}
