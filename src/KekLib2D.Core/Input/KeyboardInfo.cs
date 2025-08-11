
using Microsoft.Xna.Framework.Input;

namespace KekLib2D.Core.Input;

public class KeyboardInfo
{
    public KeyboardState PreviousState { get; private set; }
    public KeyboardState CurrentState { get; private set; }

    public KeyboardInfo()
    {
        PreviousState = Keyboard.GetState();
        CurrentState = PreviousState;
    }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key) => CurrentState.IsKeyDown(key);

    public bool IsKeyUp(Keys key) => CurrentState.IsKeyUp(key);

    public bool IsKeyPressed(Keys key) => CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);

    public bool IsKeyReleased(Keys key) => CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
}
