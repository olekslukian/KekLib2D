using KekLib3D;
using KekLib3D.Base;
using KekLib3D.Components;
using KekLib3D.Graphics;
using KekLib3D.Voxels;
using KekLib3D.Voxels.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomShot;

public class Game1 : Core3D
{
    private FpsCamera _camera;
    private PlayerWithFpsCamera _player;
    private VoxelDataManager _voxelDataManager;
    private VoxelTextureAtlas _voxelTextureAtlas;
    private VoxelMap _voxelMap;
    private VoxelRenderer _voxelRenderer;
    private Crosshair _crosshair;
    private GameSettings _gameSettings;
    public Game1() : base("BoomShot", 1280, 720, false)
    {
        IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        _gameSettings = GameSettings.FromFile("settings.ini", Content);

        _voxelDataManager = new VoxelDataManager();
        _voxelDataManager.LoadFromXml(Content, "voxels.xml");

        var requiredTextures = _voxelDataManager.GetAllUniqueTextureNames();
        _voxelTextureAtlas = new VoxelTextureAtlas(GraphicsDevice, Content, folderName: "voxel_textures", requiredTextures, textureSize: 32);



        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _crosshair?.Dispose();

        base.UnloadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

        _camera = new FpsCamera(
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            new Vector3(0, 10, 0)
        )
        {
            Fov = _gameSettings.Fov,
        };

        var basePlayer = new ControllablePlayer("user", Input)
        {
            Speed = _gameSettings.MovingSpeed,
            IsMouseGrabbed = false,
            AreControlsEnabled = true,
        };

        _player = new PlayerWithFpsCamera(basePlayer, _camera, BasicEffect)
        {
            MouseSensitivity = _gameSettings.MouseSensitivity,
        };

        var _mapGenerator = new MapGenerator(["grass"], ["stone", "crate_01"]);

        _voxelMap = _mapGenerator.GenerateMap(50, 50, 67);


        _voxelRenderer = new VoxelRenderer(GraphicsDevice);

        _crosshair = new Crosshair(GraphicsDevice, SpriteBatch)
        {
            Size = _gameSettings.CrosshairSize,
            Thickness = _gameSettings.CrosshairThickness,
            Color = new Color(_gameSettings.CrosshairColor)
        };
    }



    protected override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);


        if (_voxelMap.IsDirty)
        {
            _voxelRenderer.Build(_voxelMap, _voxelDataManager, _voxelTextureAtlas);
            _voxelMap.ClearDirty();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        BasicEffect.World = Matrix.Identity;
        _player.Draw(gameTime);

        BasicEffect.TextureEnabled = true;
        BasicEffect.VertexColorEnabled = false;

        BasicEffect.Texture = _voxelTextureAtlas.AltasTexture;

        foreach (var pass in BasicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _voxelRenderer.Draw();
        }

        BasicEffect.LightingEnabled = false;
        BasicEffect.TextureEnabled = false;
        BasicEffect.VertexColorEnabled = true;
        BasicEffect.DiffuseColor = Color.Gray.ToVector3();

        _crosshair.Draw();


        base.Draw(gameTime);
    }
}
