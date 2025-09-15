using System;
using System.Collections.Generic;
using KekLib2D.Core.Input;
using KekLib3D;
using KekLib3D.Blocks;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox;

public class Game1 : Core3D
{
    private ControllableFpsCamera _camera;
    private VertexBuffer _gridVertexBuffer;
    private VertexBuffer _blockVertexBuffer;
    private IndexBuffer _blockIndexBuffer;
    private Dictionary<Vector3, Block> _blocks = [];
    private VertexBuffer _highlightVertexBuffer;
    private IndexBuffer _highlightIndexBuffer;
    private Vector3? _highlightCellCenter;

    public Game1() : base("KekLib3D Sandbox", 1280, 720, true)
    {
        IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();

        _camera = new ControllableFpsCamera(
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            new Vector3(0, 0, 0),
            GameSettings
        );

        CreateGridPlane(100, 100, 1f);
        CreateBlockMesh();
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);
        UpdateHighlightCell();
        HandleBlockPlacement();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.Black);
        BasicEffect.World = Matrix.Identity;
        BasicEffect.DiffuseColor = Color.Gray.ToVector3();
        BasicEffect.VertexColorEnabled = true;
        BasicEffect.View = _camera.ViewMatrix;
        BasicEffect.Projection = _camera.ProjectionMatrix;

        foreach (var pass in BasicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.SetVertexBuffer(_gridVertexBuffer);
            GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, _gridVertexBuffer.VertexCount / 2);
        }

        DrawHighlightCell();

        GraphicsDevice.SetVertexBuffer(_blockVertexBuffer);
        GraphicsDevice.Indices = _blockIndexBuffer;

        foreach (var block in _blocks.Values)
        {
            BasicEffect.World = Matrix.CreateTranslation(block.Position);
            BasicEffect.DiffuseColor = block.Color.ToVector3();

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _blockIndexBuffer.IndexCount / 3);
            }
        }

        base.Draw(gameTime);
    }

    private void CreateGridPlane(int width, int height, float spacing)
    {
        var vertices = new List<VertexPositionColor>();
        var color = Color.Gray;
        float halfWidth = width * spacing / 2f;
        float halfHeight = height * spacing / 2f;

        for (int i = 0; i <= height; i++)
        {
            float z = i * spacing - halfHeight;
            vertices.Add(new VertexPositionColor(new Vector3(-halfWidth, 0, z), color));
            vertices.Add(new VertexPositionColor(new Vector3(halfWidth, 0, z), color));
        }

        for (int i = 0; i <= width; i++)
        {
            float x = i * spacing - halfWidth;
            vertices.Add(new VertexPositionColor(new Vector3(x, 0, -halfHeight), color));
            vertices.Add(new VertexPositionColor(new Vector3(x, 0, halfHeight), color));
        }

        _gridVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
        _gridVertexBuffer.SetData(vertices.ToArray());
    }

    private void CreateBlockMesh()
    {
        var (vertices, indices) = BlockMeshGenerator.CreateCube(Color.White);
        _blockVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        _blockVertexBuffer.SetData(vertices);
        _blockIndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        _blockIndexBuffer.SetData(indices);
    }

    private void HandleBlockPlacement()
    {
        if (_highlightCellCenter.HasValue)
        {

            Vector3 blockCenter = new(_highlightCellCenter.Value.X, 0.5f, _highlightCellCenter.Value.Z);

            if (Input.Mouse.IsButtonPressed(MouseButton.Left))
            {
                if (!_blocks.ContainsKey(blockCenter))
                    _blocks.Add(blockCenter, new Block(blockCenter, Color.White));
            }

            if (Input.Mouse.IsButtonPressed(MouseButton.Right))
            {
                if (_blocks.ContainsKey(blockCenter))
                    _blocks.Remove(blockCenter);
                else
                {
                    Ray ray = CastRay();
                    float closestDistance = float.MaxValue;
                    Vector3? blockToRemove = null;

                    foreach (var block in _blocks)
                    {
                        BoundingBox box = new(block.Key - Vector3.One * 0.5f, block.Key + Vector3.One * 0.5f);
                        float? distance = ray.Intersects(box);
                        if (distance.HasValue && distance.Value < closestDistance)
                        {
                            closestDistance = distance.Value;
                            blockToRemove = block.Key;
                        }
                    }

                    if (blockToRemove.HasValue)
                    {
                        _blocks.Remove(blockToRemove.Value);
                    }
                }
            }
        }
    }

    private void DrawHighlightCell()
    {
        if (_highlightVertexBuffer == null || _highlightIndexBuffer == null)
            return;

        var oldBlend = GraphicsDevice.BlendState;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

        BasicEffect.World = Matrix.Identity;
        BasicEffect.DiffuseColor = Vector3.One;

        foreach (var pass in BasicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.SetVertexBuffer(_highlightVertexBuffer);
            GraphicsDevice.Indices = _highlightIndexBuffer;
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _highlightIndexBuffer.IndexCount / 3);
        }

        GraphicsDevice.BlendState = oldBlend;
    }

    private Ray CastRay()
    {
        Vector3 nearPoint = new(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2, 0);
        Vector3 farPoint = new(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2, 1);

        Vector3 nearWorld = GraphicsDevice.Viewport.Unproject(nearPoint, _camera.ProjectionMatrix, _camera.ViewMatrix, Matrix.Identity);
        Vector3 farWorld = GraphicsDevice.Viewport.Unproject(farPoint, _camera.ProjectionMatrix, _camera.ViewMatrix, Matrix.Identity);

        Vector3 direction = Vector3.Normalize(farWorld - nearWorld);
        return new Ray(nearWorld, direction);
    }

    private void UpdateHighlightCell()
    {
        Ray ray = CastRay();
        float? dist = ray.Intersects(new Plane(Vector3.Up, 0));
        if (!dist.HasValue)
        {
            _highlightCellCenter = null;
            return;
        }

        Vector3 hit = ray.Position + ray.Direction * dist.Value;

        float cx = MathF.Floor(hit.X) + 0.5f;
        float cz = MathF.Floor(hit.Z) + 0.5f;

        Vector3 newCenter = new(cx, 0.5f, cz);

        if (!_highlightCellCenter.HasValue || newCenter != _highlightCellCenter.Value)
        {
            _highlightCellCenter = newCenter;
            RebuildHighlightQuad(newCenter);
        }
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

        _highlightVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        _highlightVertexBuffer.SetData(vertices);
        _highlightIndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        _highlightIndexBuffer.SetData(indices);
    }
}
