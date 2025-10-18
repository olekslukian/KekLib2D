
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Input;

public interface IControllable
{
    public bool IsMouseGrabbed { get; set; }
    public bool AreControlsEnabled { get; set; }
    public InputManager Input { get; }
}
