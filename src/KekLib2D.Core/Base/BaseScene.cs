using System;
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Base;

public abstract class BaseScene(Core core) : IScene
{
    protected Core Core { get; } = core;

    public virtual void Initialize()
    {
        LoadContent();
    }
    public abstract void LoadContent();
    public virtual void UnloadContent()
    {
        Core.Content.Unload();
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);

    public virtual void Dispose()
    {
        UnloadContent();
        Core.Content.Dispose();
        GC.SuppressFinalize(this);
    }

    ~BaseScene() => Dispose();
}
