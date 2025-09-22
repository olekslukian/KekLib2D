using System;
using System.Collections.Generic;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox.Rendering;

public class SandboxGrid : IDisposable
{
  private readonly VertexBuffer _vertexBuffer;
  private readonly int _primitiveCount;
  private readonly GraphicsDevice _graphicsDevice;
  private Rectangle _gridBounds;

  public SandboxGrid(GraphicsDevice graphicsDevice, int width, int height, float spacing, Color color)
  {
    _graphicsDevice = graphicsDevice;
    _gridBounds = new Rectangle(-width / 2, -height / 2, width, height);

    var verts = new List<VertexPositionColor>();
    float halfWidth = width * spacing / 2f;
    float halfHeight = height * spacing / 2f;

    for (int i = 0; i <= height; i++)
    {
      float z = i * spacing - halfHeight;
      verts.Add(new VertexPositionColor(new Vector3(-halfWidth, 0, z), color));
      verts.Add(new VertexPositionColor(new Vector3(halfWidth, 0, z), color));
    }

    for (int i = 0; i <= width; i++)
    {
      float x = i * spacing - halfWidth;
      verts.Add(new VertexPositionColor(new Vector3(x, 0, -halfHeight), color));
      verts.Add(new VertexPositionColor(new Vector3(x, 0, halfHeight), color));
    }

    _primitiveCount = verts.Count / 2;
    _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionColor), verts.Count, BufferUsage.WriteOnly);
    _vertexBuffer.SetData(verts.ToArray());
  }

  public void Draw(BasicEffect effect)
  {
    _graphicsDevice.SetVertexBuffer(_vertexBuffer);
    foreach (var pass in effect.CurrentTechnique.Passes)
    {
      pass.Apply();
      _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, _primitiveCount);
    }
  }

  public void Dispose()
  {
    _vertexBuffer?.Dispose();
    GC.SuppressFinalize(this);
  }
}
