using KekLib2D.Core;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SlimeKiller.Entities;
using System;

namespace SlimeKiller;

public class Game1 : Core
{
    private Slime _slime;
    private Player _player;
    private Tilemap _tilemap;
    private Rectangle _roomBounds;
    private Song _themeSong;
    private SoundEffect _bounceSfx;
    private SoundEffect _collectSfx;
    private SpriteFont _font;
    private Vector2 _scoreTextPosition;
    private Vector2 _scoreTextOrigin;
    private int _score;

    public Game1() : base("SlimeKiller", 1280, 720, false) { }

    protected override void Initialize()
    {
        base.Initialize();

        Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
             (int)_tilemap.TileWidth,
             (int)_tilemap.TileHeight,
             screenBounds.Width - (int)_tilemap.TileWidth * 2,
             screenBounds.Height - (int)_tilemap.TileHeight * 2
         );

        Audio.PlaySong(_themeSong);

        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _tilemap = Tilemap.FromCustomFile(Content, "Sprites/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        _themeSong = Content.Load<Song>("Audio/theme");
        _bounceSfx = Content.Load<SoundEffect>("Audio/bounce");
        _collectSfx = Content.Load<SoundEffect>("Audio/collect");

        _player = new Player(new Vector2(_roomBounds.Left, _roomBounds.Top), PlayerDirection.Forward, Input, Content);

        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        Vector2 _slimePos = new(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        _slime = new Slime(_slimePos, Content);
        _slime.OnBounce += () => Audio.PlaySoundEffect(_bounceSfx);
        _slime.OnCollected += () => Audio.PlaySoundEffect(_collectSfx);

        _font = Content.Load<SpriteFont>("fonts/04B_30");

    }

    protected override void Update(GameTime gameTime)
    {

        _player.Update(gameTime, Input);
        _slime.Update(gameTime);
        _slime.CheckIfInRoomBounds(gameTime, _roomBounds);
        _player.CheckIfInRoomBounds(_roomBounds);
        CheckPlayerSlimeCollision();

        base.Update(gameTime);
    }

    private void CheckPlayerSlimeCollision()
    {
        if (_player.CollidesWith(_slime.Bounds))
        {
            _slime.OnCollision(GraphicsDevice.PresentationParameters);
            _score += 100;
        }
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

        _tilemap.Draw(SpriteBatch);

        _player.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch);

        SpriteBatch.DrawString(
          _font,
          $"Score: {_score}",
          _scoreTextPosition,
          Color.White,
          0.0f,
          _scoreTextOrigin,
          1.0f,
          SpriteEffects.None,
          1.0f
      );

        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
