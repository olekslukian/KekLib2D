using System;
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Base;

public interface IGameObject : IDisposable
{
    public string Id { get; }
    public void Update(GameTime gameTime);
}
