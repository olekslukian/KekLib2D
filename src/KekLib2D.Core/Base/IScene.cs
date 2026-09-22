using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KekLib2D.Core.Base;

public interface IScene : IDisposable
{
    void Initialize();
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
    void LoadContent();
    void UnloadContent();
}
