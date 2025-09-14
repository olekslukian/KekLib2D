using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KekLib3D.Graphics;

public class ControllableFpsCamera(float width, float height, Vector3 position, GameSettings gameSettings) : FpsCamera(width, height, position, fov: gameSettings.Fov), IControllable
{
    private bool _firstMove = true;
    private Vector2 _lastMousePosition;
    private readonly float _speed = gameSettings.MovingSpeed;
    private readonly float _sensitivity = gameSettings.MouseSensitivity;

    public void OnInput(GameTime gameTime, InputManager input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (input.Keyboard.IsKeyDown(Keys.W))
            Position += Front * _speed * dt;
        if (input.Keyboard.IsKeyDown(Keys.S))
            Position -= Front * _speed * dt;
        if (input.Keyboard.IsKeyDown(Keys.A))
            Position -= Right * _speed * dt;
        if (input.Keyboard.IsKeyDown(Keys.D))
            Position += Right * _speed * dt;
        if (input.Keyboard.IsKeyDown(Keys.Space))
            Position += Up * _speed * dt;
        if (input.Keyboard.IsKeyDown(Keys.LeftShift))
            Position -= Up * _speed * dt;

        if (_firstMove)
        {
            _lastMousePosition = new Vector2(input.Mouse.X, input.Mouse.Y);
            _firstMove = false;
        }
        else
        {
            var dX = input.Mouse.X - _lastMousePosition.X;
            var dY = input.Mouse.Y - _lastMousePosition.Y;
            _lastMousePosition = new Vector2(input.Mouse.X, input.Mouse.Y);

            Yaw += dX * _sensitivity * dt;
            Pitch -= dY * _sensitivity * dt;
        }
    }

    public void Update(GameTime gameTime, InputManager input)
    {
        OnInput(gameTime, input);
        Update();
    }
}
