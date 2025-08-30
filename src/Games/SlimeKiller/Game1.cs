using KekLib2D.Core;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SlimeKiller.Entities;

namespace SlimeKiller;

public class Game1 : Core
{
    private Slime _slime;
    private Player _player;
    private Tilemap _tilemap;
    private Rectangle _roomBounds;
    private Song _themeSong;

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
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        TextureAtlas playerAtlas = TextureAtlas.FromFile(Content, "Sprites/player-atlas-definitions.xml");

        _tilemap = Tilemap.FromCustomFile(Content, "Sprites/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        _themeSong = Content.Load<Song>("Audio/theme");

        _player = new Player(playerAtlas, new Vector2(_roomBounds.Left, _roomBounds.Top), PlayerDirection.Forward, Input);

        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        Vector2 _slimePos = new(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        _slime = new Slime(_slimePos, Content, Audio);

    }

    protected override void Update(GameTime gameTime)
    {

        _player.Update(gameTime, Input, _slime, _roomBounds, GraphicsDevice);
        _slime.Update(gameTime, _roomBounds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

        _tilemap.Draw(SpriteBatch);

        _player.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
