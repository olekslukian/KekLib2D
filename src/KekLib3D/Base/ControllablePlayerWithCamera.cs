

using KekLib2D.Core.Base;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Base;

public class ControllablePlayerWithCamera(ControllablePlayer player, FpsCamera camera, BasicEffect effect) : IGameObject
{
    private readonly ControllablePlayer _player = player;
    private readonly FpsCamera _camera = camera;
    private readonly BasicEffect _effect = effect;
    public float MouseSensitivity { get; set; } = 100f;

    public string Id => _player.Id;

    public Vector3 Position
    {
        get => _player.Position;
        set => _player.Position = value;
    }

    public bool IsMouseGrabbed
    {
        get => _player.IsMouseGrabbed;
        set => _player.IsMouseGrabbed = value;
    }

    public bool AreControlsEnabled
    {
        get => _player.AreControlsEnabled;
        set => _player.AreControlsEnabled = value;
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
        UpdateCamera(gameTime);
        UpdatePlayerFacingDirection();
    }

    private void UpdateCamera(GameTime gameTime)
    {
        if (!AreControlsEnabled)
        {
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_player.Input.Mouse.WasMoved)
        {
            _camera.Yaw += _player.Input.Mouse.XDelta * MouseSensitivity * dt;
            _camera.Pitch -= _player.Input.Mouse.YDelta * MouseSensitivity * dt;

            if (IsMouseGrabbed)
            {
                _player.Input.Mouse.SetPosition((int)(_camera.ScreenWidth / 2), (int)(_camera.ScreenHeight / 2));
            }
        }

        _camera.Update(Position);
    }

    private void UpdatePlayerFacingDirection()
    {
        _player.Front = _camera.Front;
        _player.Right = _camera.Right;
        _player.Up = _camera.Up;
    }

    public void Draw(GameTime gameTime)
    {
        _camera.Draw(_effect);
    }
}
