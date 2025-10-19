using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Base;

public abstract class ControllableGameObject(string id, InputManager input) : IGameObject, IControllable
{
    public bool IsMouseGrabbed { get; set; }
    public bool AreControlsEnabled { get; set; }
    public InputManager Input { get; } = input;
    public string Id { get; private set; } = id;

    protected abstract void OnInput(GameTime gameTime);

    public virtual void Update(GameTime gameTime)
    {
        OnInput(gameTime);
    }
}
