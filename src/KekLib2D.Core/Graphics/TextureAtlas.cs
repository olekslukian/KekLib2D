using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib2D.Core.Graphics;

public class TextureAtlas
{
    private readonly Dictionary<string, TextureRegion> _regions;
    private readonly Dictionary<string, Animation> _animations;

    public Texture2D Texture { get; set; }

    public TextureAtlas()
    {
        _regions = [];
        _animations = [];
    }

    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = [];
        _animations = [];
    }

    public void AddRegion(string name, int x, int y, int width, int heighth) => _regions.Add(name, new TextureRegion(Texture, x, y, width, heighth));

    public TextureRegion GetRegion(string name) => _regions[name];

    public bool RemoveRegion(string name) => _regions.Remove(name);

    public void ClearRegions() => _regions.Clear();

    public void AddAnimation(string name, Animation animation) => _animations.Add(name, animation);

    public Animation GetAnimation(string name) => _animations[name];

    public bool RemoveAnimation(string name) => _animations.Remove(name);

    public void ClearAnimations() => _animations.Clear();

    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }

    /// <summary>
    /// Creates a new texture atlas based a texture atlas xml configuration file.
    /// </summary>
    /// <param name="content">The content manager used to load the texture for the atlas.</param>
    /// <param name="fileName">The path to the xml file, relative to the content root directory.</param>
    /// <returns>The texture atlas created by this method.</returns>
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using Stream stream = TitleContainer.OpenStream(filePath);
        using XmlReader reader = XmlReader.Create(stream);
        XDocument doc = XDocument.Load(reader);
        XElement root = doc.Root;

        string texturePath = root.Element("Texture").Value;
        atlas.Texture = content.Load<Texture2D>(texturePath);

        var regions = root.Element("Regions")?.Elements("Region");

        if (regions != null)
        {
            foreach (var region in regions)
            {
                string name = region.Attribute("name")?.Value;
                int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                if (!string.IsNullOrEmpty(name))
                {
                    atlas.AddRegion(name, x, y, width, height);
                }
            }
        }

        var animationElements = root.Element("Animations").Elements("Animation");

        if (animationElements != null)
        {
            foreach (var animationElement in animationElements)
            {
                string name = animationElement.Attribute("name")?.Value;
                float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                List<TextureRegion> frames = [];

                var frameElements = animationElement.Elements("Frame");

                if (frameElements != null)
                {
                    foreach (var frameElement in frameElements)
                    {
                        string regionName = frameElement.Attribute("region").Value;
                        TextureRegion region = atlas.GetRegion(regionName);
                        frames.Add(region);
                    }
                }

                Animation animation = new(frames, delay);
                atlas.AddAnimation(name, animation);
            }
        }

        return atlas;
    }
}
