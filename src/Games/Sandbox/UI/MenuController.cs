using KekLib2D.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Sandbox.UI;

public class MenuController(InputManager input)
{
    public bool IsMenuShown { get; private set; } = false;
    private InputManager _input = input;

    public void Update()
    {
        if (_input.Keyboard.IsKeyPressed(Keys.Tab))
        {
            IsMenuShown = !IsMenuShown;
        }
    }

}
