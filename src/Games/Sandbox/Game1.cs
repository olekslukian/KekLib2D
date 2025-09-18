using System;
using System.Collections.Generic;
using KekLib2D.Core.Input;
using KekLib3D;
using KekLib3D.Blocks;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;

namespace Sandbox;

public class Game1 : Core3D
{
    private ControllableFpsCamera _camera;
    private VertexBuffer _blockVertexBuffer;
    private IndexBuffer _blockIndexBuffer;
    private Dictionary<Vector3, Block> _blocks = [];
    private SandboxGrid _grid;

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

        _grid = new SandboxGrid(GraphicsDevice, BasicEffect, 100, 100, 1f, Color.Gray, _camera);

        CreateBlockMesh();
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);
        _grid.Update();
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

        _grid.Draw(BasicEffect);

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
        if (_grid.Highlight.CellCenter.HasValue)
        {

            Vector3 blockCenter = new(_grid.Highlight.CellCenter.Value.X, 0.5f, _grid.Highlight.CellCenter.Value.Z);

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
                    Ray ray = Raycaster.CastRay(GraphicsDevice, _camera);
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
}
