using KekLib2D.Core;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SlimeKiller;

public class Game1 : Core
{
    private const float _velocity = 200f;

    private AnimatedSprite _player;
    private AnimatedSprite _slime;
    private Vector2 _playerPos;
    private Vector2 _slimePos;

    public Game1() : base("MGSpriteDraw", 1280, 720, false)
    {


    }

    protected override void Initialize()
    {
        base.Initialize();

    }

    protected override void LoadContent()
    {
        base.LoadContent();

        TextureAtlas playerAtlas = TextureAtlas.FromFile(Content, "Sprites/player-atlas-definitions.xml");
        TextureAtlas slimeAtlas = TextureAtlas.FromFile(Content, "Sprites/slime-atlas-definitions.xml");

        _player = playerAtlas.CreateAnimatedSprite("player-walk-forward");
        _player.Scale = new(4.0f, 4.0f);
        _player.LayerDepth = 1f;
        _slime = slimeAtlas.CreateAnimatedSprite("slime-idle");
        _slime.Scale = new(4.0f, 4.0f);

        _playerPos = new Vector2((Window.ClientBounds.Width * 0.5f) - (_player.Width * 0.5f), (Window.ClientBounds.Height * 0.5f) - (_player.Height * 0.5f));
        _slimePos = new Vector2(_playerPos.X + 50, _playerPos.Y);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (keyboard.IsKeyDown(Keys.W))
        {
            _playerPos.Y -= _velocity * dt;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            _playerPos.Y += _velocity * dt;
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            _playerPos.X -= _velocity * dt;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            _playerPos.X += _velocity * dt;
        }

        _player.Update(gameTime);
        _slime.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

        _player.Draw(SpriteBatch, _playerPos);

        _slime.Draw(SpriteBatch, _slimePos);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
