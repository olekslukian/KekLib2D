
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib2D.Core;

public class Core : Game
{
    internal static Core s_instance;
    public static Core Instance => s_instance;
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static new ContentManager Content { get; private set; }

    public Core(string title, int width, int height, bool fullScreen)
    {
        if (s_instance != null)
            throw new System.Exception("Core instance already exists.");

        s_instance = this;

        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
            IsFullScreen = fullScreen
        };
        Graphics.ApplyChanges();

        Window.Title = title;
        Content = base.Content;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
}