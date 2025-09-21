using KekLib2D.Core.Input;
using KekLib3D;
using KekLib3D.Voxels;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Voxels.Utils;

namespace Sandbox;

public class Game1 : Core3D
{
    private ControllableFpsCamera _camera;
    private SandboxGrid _grid;
    private VoxelMap _voxelMap;
    private VoxelRenderer _voxelRenderer;
    private VoxelHighlight _voxelHighlight;
    private PickResult _lastPick;

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

        _voxelMap = new VoxelMap();
        _voxelRenderer = new VoxelRenderer(GraphicsDevice, _voxelMap);
        _voxelHighlight = new VoxelHighlight(GraphicsDevice, BasicEffect);

        _voxelMap.Set(new Int3(0, 0, 0), 1);
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);
        _grid.Update();

        HandleVoxelPlacement();
        _voxelRenderer.RebuildIfDirty();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.Black);

        BasicEffect.World = Matrix.Identity;
        BasicEffect.View = _camera.ViewMatrix;
        BasicEffect.Projection = _camera.ProjectionMatrix;
        BasicEffect.VertexColorEnabled = true;
        BasicEffect.DiffuseColor = Color.Gray.ToVector3();

        _grid.Draw(BasicEffect);
        _voxelRenderer.Draw(BasicEffect);
        _voxelHighlight.Draw();

        base.Draw(gameTime);
    }

    private void HandleVoxelPlacement()
    {
        Ray ray = Raycaster.CastRay(GraphicsDevice, _camera);

        _lastPick = VoxelPicker.Pick(ray, _voxelMap);

        if (_lastPick.Type == HitType.Block && _voxelMap.Has(_lastPick.HitVoxel))
        {
            _voxelHighlight.ShowAt(_lastPick.HitVoxel);
        }
        else
        {
            _voxelHighlight.Hide();
        }

        if (Input.Mouse.IsButtonPressed(MouseButton.Left))
        {
            if (_lastPick.Type == HitType.Block || _lastPick.Type == HitType.Ground)
            {
                var pos = _lastPick.PlacePosition;
                if (_voxelMap.Has(pos))
                {
                    _voxelMap.Set(pos, 1);
                }
            }
        }

        if (Input.Mouse.IsButtonPressed(MouseButton.Right))
        {
            if (_lastPick.Type == HitType.Block)
            {
                _voxelMap.Remove(_lastPick.HitVoxel);
            }
        }
    }


}
