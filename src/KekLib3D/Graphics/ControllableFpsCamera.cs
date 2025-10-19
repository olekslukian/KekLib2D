// using KekLib2D.Core.Input;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Input;

// namespace KekLib3D.Graphics;

// public class ControllableFpsCamera(float width, float height, Vector3 position) : FpsCamera(width, height, position), IControllable
// {
//     public bool IsMouseGrabbed { get; set; } = true;
//     public bool AreControlsEnabled { get; set; } = true;
//     public float Speed { get; set; } = 150f;
//     public float MouseSensitivity { get; set; } = 100f;

//     private void OnInput(GameTime gameTime, InputManager input)
//     {
//         if (!AreControlsEnabled)
//             return;

//         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

//         if (input.Keyboard.IsKeyDown(Keys.W))
//             Position += Front * Speed * dt;
//         if (input.Keyboard.IsKeyDown(Keys.S))
//             Position -= Front * Speed * dt;
//         if (input.Keyboard.IsKeyDown(Keys.A))
//             Position -= Right * Speed * dt;
//         if (input.Keyboard.IsKeyDown(Keys.D))
//             Position += Right * Speed * dt;
//         if (input.Keyboard.IsKeyDown(Keys.Space))
//             Position = new Vector3(Position.X, Position.Y + Speed * dt, Position.Z);
//         if (input.Keyboard.IsKeyDown(Keys.LeftShift))
//             Position = new Vector3(Position.X, Position.Y - Speed * dt, Position.Z);

//         if (input.Mouse.WasMoved)
//         {
//             Yaw += input.Mouse.XDelta * MouseSensitivity * dt;
//             Pitch -= input.Mouse.YDelta * MouseSensitivity * dt;

//             if (IsMouseGrabbed)
//             {
//                 input.Mouse.SetPosition((int)(_screenWidth / 2), (int)(_screenHeight / 2));
//             }
//         }
//     }

//     public void Update(GameTime gameTime, InputManager input)
//     {
//         OnInput(gameTime, input);
//         Update();
//     }

// }
