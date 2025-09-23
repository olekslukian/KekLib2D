using System;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public class VoxelHighlight(GraphicsDevice graphicsDevice, BasicEffect effect) : IDisposable
{
    readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    readonly BasicEffect _effect = effect;
    VertexBuffer _vertexBuffer;
    Vector3 _center;

    public bool Visible { get; private set; }

    public void ShowAt(Int3 pos)
    {
        _center = new(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f);
        Rebuild();
        Visible = true;
    }

    public void Hide() => Visible = false;

    void Rebuild()
    {
        float epsilon = 0.001f;

        var c = _center;
        Vector3[] points = [
            c + new Vector3(-0.5f, -0.5f + epsilon, -0.5f),
            c + new Vector3(0.5f, -0.5f + epsilon, -0.5f),
            c + new Vector3(0.5f, -0.5f + epsilon, 0.5f),
            c + new Vector3(-0.5f, -0.5f + epsilon, 0.5f),
            c + new Vector3(-0.5f, 0.5f + epsilon, -0.5f),
            c + new Vector3(0.5f, 0.5f + epsilon, -0.5f),
            c + new Vector3(0.5f, 0.5f + epsilon, 0.5f),
            c + new Vector3(-0.5f, 0.5f + epsilon, 0.5f),
        ];

        int[] edges = [
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        ];

        var verts = new VertexPositionColor[edges.Length];
        var color = new Color(255, 255, 0, 160);
        for (int i = 0; i < edges.Length; i++)
        {
            verts[i] = new VertexPositionColor(points[edges[i]], color);
        }

        _vertexBuffer?.Dispose();
        _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(verts);
    }

    public void Draw()
    {
        if (!Visible || _vertexBuffer == null) return;

        var old = _graphicsDevice.RasterizerState;
        _graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
        _graphicsDevice.SetVertexBuffer(_vertexBuffer);

        _effect.VertexColorEnabled = true;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, _vertexBuffer.VertexCount / 2);
        }
        _graphicsDevice.RasterizerState = old;
    }

    public void Dispose()
    {
        _vertexBuffer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
