
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Input;

public interface IControllable
{
    public void OnInput(GameTime gameTime, InputManager input);
}
