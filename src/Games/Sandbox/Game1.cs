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
        if (Input.Mouse.IsButtonPressed(MouseButton.Left))
        {
            Ray ray = CastRay();

            float? distance = ray.Intersects(new Plane(Vector3.Up, 0));
            if (distance.HasValue)
            {
                Vector3 worldPos = ray.Position + ray.Direction * distance.Value;
                Vector3 snappedPos = new(
                    (float)Math.Round(worldPos.X) + 0.5f,
                    (float)Math.Round(worldPos.Y) + 0.5f,
                    (float)Math.Round(worldPos.Z) + 0.5f
                );

                if (!_blocks.ContainsKey(snappedPos))
                {
                    _blocks.Add(snappedPos, new Block(snappedPos, Color.White));
                }
            }
        }

        if (Input.Mouse.IsButtonPressed(MouseButton.Right))
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

    private Ray CastRay()
    {
        Vector3 nearPoint = new(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2, 0);
        Vector3 farPoint = new(GraphicsDevice.PresentationParameters.BackBufferWidth / 2, GraphicsDevice.PresentationParameters.BackBufferHeight / 2, 1);

        Vector3 nearWorld = GraphicsDevice.Viewport.Unproject(nearPoint, _camera.ProjectionMatrix, _camera.ViewMatrix, Matrix.Identity);
        Vector3 farWorld = GraphicsDevice.Viewport.Unproject(farPoint, _camera.ProjectionMatrix, _camera.ViewMatrix, Matrix.Identity);

        Vector3 direction = Vector3.Normalize(farWorld - nearWorld);
        return new Ray(nearWorld, direction);
    }
}
