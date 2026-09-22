namespace KekLib3D.Voxels.Serialization;

public struct VoxelData
{
    public Position3D Position { get; set; }
    public string Id { get; set; }
    public VoxelTexture Texture { get; set; }
}
