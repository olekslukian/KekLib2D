using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public class VoxelRenderer(GraphicsDevice graphicsDevice, VoxelMap map)
{
    readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    readonly VoxelMap _map = map;
    VertexBuffer _vertexBuffer;
    IndexBuffer _indexBuffer;
    int _primCount;

    public void RebuildIfDirty()
    {
        if (!_map.IsDirty) return;

        VoxelMesher.Build(_map, out var vertices, out var indices);

        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        if (vertices.Length == 0)
        {
            _vertexBuffer = null;
            _indexBuffer = null;
            _primCount = 0;
            return;
        }

        _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColorNormal), vertices.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(vertices);
        _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(indices);

        _primCount = indices.Length / 3;
        _map.ClearDirty();
    }

    public void Draw(BasicEffect effect)
    {
        if (_vertexBuffer == null || _indexBuffer == null || _primCount == 0) return;

        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primCount);
        }
    }
}
