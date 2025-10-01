using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public class VoxelRenderer(GraphicsDevice graphicsDevice)
{
    readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    VertexBuffer _vertexBuffer;
    IndexBuffer _indexBuffer;
    int _primCount;

    public void Build(VoxelMap map, VoxelDataManager dataManager, VoxelTextureAtlas atlas)
    {

        VoxelMesher.Build(map, dataManager, atlas, out var vertices, out var indices);

        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        if (vertices.Length == 0)
        {
            _primCount = 0;
            return;
        }

        _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(vertices);
        _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(indices);

        _primCount = indices.Length / 3;
    }

    public void Draw()
    {
        if (_primCount == 0) return;

        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primCount);
    }
}
