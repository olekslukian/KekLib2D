using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Voxels.Rendering;

public class VoxelTextureAtlas
{
    public Texture2D AltasTexture { get; private set; }
    private readonly Vector2 _textureSizeInAtlasUv;
    private readonly Dictionary<string, Vector2> _textureUvStart = [];

    public VoxelTextureAtlas(GraphicsDevice graphicsDevice, ContentManager content, string folderName, List<string> textureNames, int textureSize = 16)
    {
        var loadedTextures = new Dictionary<string, Texture2D>();

        foreach (var name in textureNames)
        {
            loadedTextures[name] = content.Load<Texture2D>($"{folderName}/{name}");
        }

        int texturesPerRow = (int)Math.Ceiling(Math.Sqrt(textureNames.Count));
        int atlasDim = texturesPerRow * textureSize;
        _textureSizeInAtlasUv = new Vector2((float)textureSize / atlasDim);

        var renderTarget = new RenderTarget2D(graphicsDevice, atlasDim, atlasDim);
        using (var spriteBatch = new SpriteBatch(graphicsDevice))
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int index = 0;
            foreach (var (name, texture) in loadedTextures)
            {
                int x = index % texturesPerRow * textureSize;
                int y = index / texturesPerRow * textureSize;
                spriteBatch.Draw(texture, new Rectangle(x, y, textureSize, textureSize), Color.White);

                _textureUvStart[name] = new Vector2((float)x / atlasDim, (float)y / atlasDim);
                index++;
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

        }

        AltasTexture = renderTarget;
    }

    public Vector2 GetAtlasUv(string textureName, Vector2 faceUv)
    {
        if (_textureUvStart.TryGetValue(textureName, out var uvStart))
        {
            return uvStart + faceUv * _textureSizeInAtlasUv;
        }

        return Vector2.Zero;
    }

}
