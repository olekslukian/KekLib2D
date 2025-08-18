using KekLib2D.Core;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeKiller.GameObjects;

namespace SlimeKiller;

public class Game1 : Core
{
    private Slime _slime;
    private Player _player;

    public Game1() : base("SlimeKiller", 1280, 720, false) { }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        TextureAtlas playerAtlas = TextureAtlas.FromFile(Content, "Sprites/player-atlas-definitions.xml");
        TextureAtlas slimeAtlas = TextureAtlas.FromFile(Content, "Sprites/slime-atlas-definitions.xml");

        _player = new Player(playerAtlas, new Vector2(100, 100), PlayerDirection.Forward);
        _slime = new Slime(slimeAtlas, new Vector2(200, 200));

    }

    protected override void Update(GameTime gameTime)
    {

        _player.Update(gameTime, Input, ScreenBounds, _slime, GraphicsDevice);
        _slime.Update(gameTime, ScreenBounds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

        _player.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
