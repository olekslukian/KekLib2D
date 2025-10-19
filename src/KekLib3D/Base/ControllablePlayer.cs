using KekLib2D.Core.Base;
using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KekLib3D.Base;

public class ControllablePlayer(string id, InputManager input) : ControllableGameObject(id, input), IGameObject3D
{
    public float Speed { get; set; } = 150f;
    public Vector3 Position { get; set; } = new Vector3(0, 10, 0);
    public Vector3 Rotation { get; set; } = Vector3.Zero;
    public Vector3 Scale { get; set; } = Vector3.One;
    public Vector3 Up { get; set; }
    public Vector3 Front { get; set; }
    public Vector3 Right { get; set; }

    protected override void OnInput(GameTime gameTime)
    {
        if (!AreControlsEnabled)
            return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Input.Keyboard.IsKeyDown(Keys.W))
            Position += Front * Speed * dt;
        if (Input.Keyboard.IsKeyDown(Keys.S))
            Position -= Front * Speed * dt;
        if (Input.Keyboard.IsKeyDown(Keys.A))
            Position -= Right * Speed * dt;
        if (Input.Keyboard.IsKeyDown(Keys.D))
            Position += Right * Speed * dt;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
            Position = new Vector3(Position.X, Position.Y + Speed * dt, Position.Z);
        if (Input.Keyboard.IsKeyDown(Keys.LeftShift))
            Position = new Vector3(Position.X, Position.Y - Speed * dt, Position.Z);
    }
}

