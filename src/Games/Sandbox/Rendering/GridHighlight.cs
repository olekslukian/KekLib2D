using System;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox.Rendering;

public class GridHighlight(GraphicsDevice graphicsDevice, BasicEffect effect, FpsCamera camera, Rectangle gridBounds) : IDisposable
{
  public Vector3? CellCenter { get; private set; }
  private VertexBuffer _vertexBuffer;
  private IndexBuffer _indexBuffer;
  private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
  private readonly BasicEffect Effect = effect;
  private readonly FpsCamera _camera = camera;
  private Rectangle _gridBounds = gridBounds;

  public void Update()
  {
    Ray ray = Raycaster.CastRay(_graphicsDevice, _camera);

    float? dist = ray.Intersects(new Plane(Vector3.Up, 0));
    if (!dist.HasValue)
    {
      CellCenter = null;
      return;
    }

    Vector3 hit = ray.Position + ray.Direction * dist.Value;

    if (!_gridBounds.Contains((int)hit.X, (int)hit.Z))
    {
      CellCenter = null;
      return;
    }

    float cx = MathF.Floor(hit.X) + 0.5f;
    float cz = MathF.Floor(hit.Z) + 0.5f;

    Vector3 newCenter = new(cx, 0.5f, cz);

    if (!CellCenter.HasValue || newCenter != CellCenter.Value)
    {
      CellCenter = newCenter;
      RebuildHighlightQuad(newCenter);
    }
  }
  public void Draw()
  {
    if (_vertexBuffer == null || _indexBuffer == null)
      return;

    var oldBlend = _graphicsDevice.BlendState;
    _graphicsDevice.BlendState = BlendState.AlphaBlend;

    Effect.World = Matrix.Identity;
    Effect.DiffuseColor = Vector3.One;

    foreach (var pass in Effect.CurrentTechnique.Passes)
    {
      pass.Apply();
      _graphicsDevice.SetVertexBuffer(_vertexBuffer);
      _graphicsDevice.Indices = _indexBuffer;
      _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexBuffer.IndexCount / 3);
    }

    _graphicsDevice.BlendState = oldBlend;
  }

  private void RebuildHighlightQuad(Vector3 center)
  {
    float y = 0.001f;
    float half = 0.5f;

    var v0 = new VertexPositionColor(new Vector3(center.X - half, y, center.Z - half), new Color(160, 160, 160, 80));
    var v1 = new VertexPositionColor(new Vector3(center.X + half, y, center.Z - half), new Color(160, 160, 160, 80));
    var v2 = new VertexPositionColor(new Vector3(center.X + half, y, center.Z + half), new Color(160, 160, 160, 80));
    var v3 = new VertexPositionColor(new Vector3(center.X - half, y, center.Z + half), new Color(160, 160, 160, 80));

    VertexPositionColor[] vertices = [v0, v1, v2, v3];
    short[] indices = [0, 1, 2, 0, 2, 3];

    _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
    _vertexBuffer.SetData(vertices);
    _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
    _indexBuffer.SetData(indices);
  }

  public void Dispose()
  {
    _vertexBuffer?.Dispose();
    _indexBuffer?.Dispose();
    GC.SuppressFinalize(this);
  }
}
