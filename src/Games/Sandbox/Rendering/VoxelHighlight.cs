using System;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox.Rendering;

public class VoxelHighlight(GraphicsDevice graphicsDevice, BasicEffect effect) : IDisposable
{
    const float epsilon = 0.001f;
    readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    readonly BasicEffect _effect = effect;
    VertexBuffer _vertexBuffer;

    public bool Visible { get; private set; }

    public void ShowAt(Int3 pos, Vector3? faceNormal)
    {
        Rebuild(pos, faceNormal);
        Visible = true;
    }

    public void Hide() => Visible = false;

    void Rebuild(Int3 pos, Vector3? faceNormal)
    {
        Vector3 center = new(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f);
        Vector3[] points;

        if (faceNormal.HasValue)
        {
            points = GetFacePoints(center, faceNormal.Value);
        }
        else
        {
            points = GetGroundSquare(pos);
        }

        int[] edges = [0, 1, 1, 2, 2, 3, 3, 0];
        var verts = new VertexPositionColor[edges.Length];

        for (int i = 0; i < edges.Length; i++)
        {
            int edge = edges[i];
            verts[i] = new VertexPositionColor(points[edge], Color.YellowGreen);
        }

        _vertexBuffer?.Dispose();
        _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(verts);
    }

    public void Draw()
    {
        if (!Visible || _vertexBuffer == null) return;

        var oldRasterizer = _graphicsDevice.RasterizerState;
        var oldDepthStencil = _graphicsDevice.DepthStencilState;

        _graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
        _graphicsDevice.DepthStencilState = DepthStencilState.Default;
        _graphicsDevice.SetVertexBuffer(_vertexBuffer);

        _effect.VertexColorEnabled = true;
        _effect.LightingEnabled = false;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, _vertexBuffer.VertexCount / 2);
        }

        _graphicsDevice.RasterizerState = oldRasterizer;
        _graphicsDevice.DepthStencilState = oldDepthStencil;
    }

    public void Dispose()
    {
        _vertexBuffer?.Dispose();
        GC.SuppressFinalize(this);
    }

    private static Vector3[] GetFacePoints(Vector3 center, Vector3 normal)
    {
        if (normal == Vector3.Forward)
        {
            return [
                center + new Vector3(-0.5f, -0.5f, 0.5f - epsilon),
                center + new Vector3(0.5f, -0.5f, 0.5f - epsilon),
                center + new Vector3(0.5f, 0.5f, 0.5f - epsilon),
                center + new Vector3(-0.5f, 0.5f, 0.5f - epsilon)
            ];
        }
        else if (normal == Vector3.Backward)
        {
            return [
                center + new Vector3(0.5f, -0.5f, -0.5f + epsilon),
                center + new Vector3(-0.5f, -0.5f, -0.5f + epsilon),
                center + new Vector3(-0.5f, 0.5f, -0.5f + epsilon),
                center + new Vector3(0.5f, 0.5f, -0.5f + epsilon)
            ];
        }
        else if (normal == Vector3.Right)
        {
            return [
                center + new Vector3(-0.5f + epsilon, -0.5f, 0.5f),
                center + new Vector3(-0.5f + epsilon, -0.5f, -0.5f),
                center + new Vector3(-0.5f + epsilon, 0.5f, -0.5f),
                center + new Vector3(-0.5f + epsilon, 0.5f, 0.5f)
            ];
        }
        else if (normal == Vector3.Left)
        {
            return [
                center + new Vector3(0.5f - epsilon, -0.5f, -0.5f),
                center + new Vector3(0.5f - epsilon, -0.5f, 0.5f),
                center + new Vector3(0.5f - epsilon, 0.5f, 0.5f),
                center + new Vector3(0.5f - epsilon, 0.5f, -0.5f)
            ];
        }
        else if (normal == Vector3.Up)
        {
            return [
                center + new Vector3(-0.5f, -0.5f + epsilon, 0.5f),
                center + new Vector3(0.5f, -0.5f + epsilon, 0.5f),
                center + new Vector3(0.5f, -0.5f + epsilon, -0.5f),
                center + new Vector3(-0.5f, -0.5f + epsilon, -0.5f)
            ];
        }
        else
        {
            return [
                center + new Vector3(-0.5f, 0.5f - epsilon, -0.5f),
                center + new Vector3(0.5f, 0.5f - epsilon, -0.5f),
                center + new Vector3(0.5f, 0.5f - epsilon, 0.5f),
                center + new Vector3(-0.5f, 0.5f - epsilon, 0.5f)
            ];
        }
    }

    private static Vector3[] GetGroundSquare(Int3 pos)
    {
        return [
            new Vector3(pos.X, epsilon, pos.Z),
            new Vector3(pos.X + 1, epsilon, pos.Z),
            new Vector3(pos.X + 1, epsilon, pos.Z + 1),
            new Vector3(pos.X, epsilon, pos.Z + 1)
        ];
    }
}
