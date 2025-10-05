
using KekLib2D.Core.Input;
using KekLib3D.Graphics;
using KekLib3D.Voxels;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Voxels.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;

namespace Sandbox;

public class VoxelController(InputManager input, VoxelHighlight voxelHighlight, VoxelMap voxelMap, GraphicsDevice graphicsDevice, FpsCamera camera, SandboxGrid grid)
{
  public bool IsEnabled { get; set; } = true;
  private const float InteractionDelay = 0.12f;
  private readonly InputManager _input = input;
  private readonly VoxelHighlight _highlight = voxelHighlight;
  private readonly VoxelMap _voxelMap = voxelMap;
  private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
  private readonly FpsCamera _camera = camera;
  private readonly SandboxGrid _grid = grid;
  private PickResult _lastPick;
  private float _timer = 0f;

  public void Update(GameTime gameTime, ushort selectedVoxelId)
  {
    if (!IsEnabled)
      return;

    _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

    Ray ray = Raycaster.CastRay(_graphicsDevice, _camera);

    if (ray.Intersects(_grid.Bounds) == null)
    {
      _highlight.Hide();
      return;
    }

    _lastPick = VoxelPicker.Pick(ray, _voxelMap);

    if (_lastPick.Type == HitType.Block || _lastPick.Type == HitType.Ground)
    {
      _highlight.ShowAt(_lastPick.PlacePosition, _lastPick.FaceNormal.ToVector3());
    }
    else
    {
      _highlight.Hide();
    }

    if (_input.Mouse.IsButtonDown(MouseButton.Left) && _timer <= 0f)
    {
      PlaceBlock(selectedVoxelId);
    }

    if (_input.Mouse.IsButtonDown(MouseButton.Right) && _timer <= 0f)
    {
      RemoveBlock();
    }
  }

  private void PlaceBlock(ushort selectedVoxelId)
  {
    if (_lastPick.Type == HitType.Block || _lastPick.Type == HitType.Ground)
    {
      var pos = _lastPick.PlacePosition;
      if (!_voxelMap.Has(pos))
      {
        _voxelMap.Set(pos, selectedVoxelId);
        _timer = InteractionDelay;
      }
    }

  }

  private void RemoveBlock()
  {
    if (_lastPick.Type == HitType.Block)
    {
      _voxelMap.Remove(_lastPick.HitVoxel);
      _timer = InteractionDelay;
    }
  }

}
