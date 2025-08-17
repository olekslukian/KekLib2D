using KekLib2D.Core;
using KekLib2D.Core.Graphics;
using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SlimeKiller;

public class Game1 : Core
{
    private AnimatedSprite _slime;
    private Player _player;
    private Vector2 _slimePos;

    public Game1() : base("MGSpriteDraw", 1280, 720, false) { }

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

        _slime = slimeAtlas.CreateAnimatedSprite("slime-idle");
        _slime.Scale = new(4.0f, 4.0f);

        _slimePos = new Vector2(200, 200);
    }

    protected override void Update(GameTime gameTime)
    {
        _player.Update(gameTime, Input);
        _slime.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

        _player.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch, _slimePos);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
