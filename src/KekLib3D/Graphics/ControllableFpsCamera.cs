using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KekLib3D.Graphics;

public class ControllableFpsCamera(float width, float height, Vector3 position, GameSettings gameSettings) : FpsCamera(width, height, position, fov: gameSettings.Fov), IControllable
{
    public bool IsMouseGrabbed { get; set; } = true;
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
            Position = new Vector3(Position.X, Position.Y + _speed * dt, Position.Z);
        if (input.Keyboard.IsKeyDown(Keys.LeftShift))
            Position = new Vector3(Position.X, Position.Y - _speed * dt, Position.Z);

        if (input.Mouse.WasMoved)
        {
            Yaw += input.Mouse.XDelta * _sensitivity * dt;
            Pitch -= input.Mouse.YDelta * _sensitivity * dt;

            if (IsMouseGrabbed)
            {
                input.Mouse.SetPosition((int)(_screenWidth / 2), (int)(_screenHeight / 2));
            }
        }
    }


    public void Update(GameTime gameTime, InputManager input)
    {
        OnInput(gameTime, input);
        Update();
    }
}
