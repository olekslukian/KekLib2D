using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Components;

public class Crosshair : IDisposable
{
  public Color Color { get; set; } = Color.Gray;
  public int Size { get; set; } = 20;
  public int Thickness { get; set; } = 2;
  private readonly GraphicsDevice _graphicsDevice;
  private readonly SpriteBatch _spriteBatch;
  private readonly Texture2D _pixelTexture;

  public Crosshair(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
  {
    _graphicsDevice = graphicsDevice;
    _spriteBatch = spriteBatch;

    _pixelTexture = new Texture2D(_graphicsDevice, 1, 1);
    _pixelTexture.SetData([Color]);
  }

  public void Draw()
  {
    var viewport = _graphicsDevice.Viewport;
    var center = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

    _spriteBatch.Begin();


    _spriteBatch.Draw(_pixelTexture,
        new Rectangle((int)(center.X - Size / 2f), (int)(center.Y - Thickness / 2f), Size, Thickness),
        Color);

    _spriteBatch.Draw(_pixelTexture,
        new Rectangle((int)(center.X - Thickness / 2f), (int)(center.Y - Size / 2f), Thickness, Size),
        Color);

    _spriteBatch.End();
  }

  public void Dispose()
  {
    _pixelTexture?.Dispose();
    _spriteBatch?.Dispose();
    GC.SuppressFinalize(this);
  }

}
