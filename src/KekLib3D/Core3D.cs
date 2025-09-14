using KekLib2D.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D;

public class Core3D : Core
{
    public static BasicEffect BasicEffect { get; private set; }

    public Core3D(string title, int width, int height, bool fullScreen)
        : base(title, width, height, fullScreen)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        BasicEffect = new BasicEffect(GraphicsDevice)
        {
            VertexColorEnabled = true,
            TextureEnabled = false,
            LightingEnabled = false,
            Alpha = 1.0f
        };
    }

    protected override void LoadContent()
    {

    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}
