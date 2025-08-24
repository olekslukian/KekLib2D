using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib2D.Core.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;
    private readonly int[] _tiles;
    public int Rows { get; }
    public int Columns { get; }
    public int Count { get; }
    public Vector2 Scale { get; set; }
    public float TileWidth => _tileset.TileWidth * Scale.X;
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int[Count];
    }

    public void SetTile(int index, int tilesetId) => _tiles[index] = tilesetId;

    public void SetTile(int column, int row, int tilesetId)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetId);
    }

    public TextureRegion GetTile(int index)
    {
        int tilesetId = _tiles[index];
        return _tileset.GetTile(tilesetId);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int tileSetIndex = _tiles[i];
            TextureRegion tile = _tileset.GetTile(tileSetIndex);

            int x = i % Columns;
            int y = i / Columns;

            Vector2 position = new(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
        }

    }

    public static Tilemap FromCustomFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using Stream stream = TitleContainer.OpenStream(filePath);
        using XmlReader reader = XmlReader.Create(stream);
        XDocument doc = XDocument.Load(reader);
        XElement root = doc.Root;

        XElement tilesetElement = root.Element("Tileset");

        string regionAttribute = tilesetElement.Attribute("region").Value;
        string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        int x = int.Parse(split[0]);
        int y = int.Parse(split[1]);
        int width = int.Parse(split[2]);
        int height = int.Parse(split[3]);

        int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
        int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
        string contentPath = tilesetElement.Value;

        Texture2D texture = content.Load<Texture2D>(contentPath);

        TextureRegion textureRegion = new(texture, x, y, width, height);

        Tileset tileset = new(textureRegion, tileWidth, tileHeight);

        XElement tilesElement = root.Element("Tiles");

        string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

        Tilemap tilemap = new(tileset, columnCount, rows.Length);

        for (int row = 0; row < rows.Length; row++)
        {
            string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (int column = 0; column < columnCount; column++)
            {
                int tilesetIndex = int.Parse(columns[column]);

                TextureRegion region = tileset.GetTile(tilesetIndex);

                tilemap.SetTile(column, row, tilesetIndex);
            }
        }

        return tilemap;
    }

}
