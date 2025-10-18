using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Base;

public interface IGameObject
{
    public string Id { get; }
    public void Update(GameTime gameTime);
}
