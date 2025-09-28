
using KekLib2D.Core.Input;
using KekLib3D.Graphics;
using KekLib3D.Voxels;
using KekLib3D.Voxels.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;

namespace Sandbox;

public class VoxelController(InputManager input, VoxelHighlight voxelHighlight, VoxelMap voxelMap, GraphicsDevice graphicsDevice, FpsCamera camera, SandboxGrid grid)
{
  private readonly InputManager _input = input;
  private readonly VoxelHighlight _highlight = voxelHighlight;
  private readonly VoxelMap _voxelMap = voxelMap;
  private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
  private readonly FpsCamera _camera = camera;
  private readonly SandboxGrid _grid = grid;
  private PickResult _lastPick;

  public void Update()
  {
    Ray ray = Raycaster.CastRay(_graphicsDevice, _camera);

    if (ray.Intersects(_grid.Bounds) == null)
    {
      _highlight.Hide();
      return;
    }

    _lastPick = VoxelPicker.Pick(ray, _voxelMap);

    if (_lastPick.Type == HitType.Block || _lastPick.Type == HitType.Ground)
    {
      _highlight.ShowAt(_lastPick.PlacePosition);
    }
    else
    {
      _highlight.Hide();
    }

    if (_input.Mouse.IsButtonPressed(MouseButton.Left))
    {
      if (_lastPick.Type == HitType.Block || _lastPick.Type == HitType.Ground)
      {
        var pos = _lastPick.PlacePosition;
        if (!_voxelMap.Has(pos))
        {
          _voxelMap.Set(pos, 1);
        }
      }
    }

    if (_input.Mouse.IsButtonPressed(MouseButton.Right))
    {
      if (_lastPick.Type == HitType.Block)
      {
        _voxelMap.Remove(_lastPick.HitVoxel);
      }
    }
  }

}
