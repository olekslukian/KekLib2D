using KekLib3D;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Components;
using KekLib3D.Voxels;

namespace Sandbox;

public class Game1 : Core3D
{
    private ControllableFpsCamera _camera;
    private SandboxGrid _grid;
    private VoxelMap _voxelMap;
    private VoxelRenderer _voxelRenderer;
    private VoxelHighlight _voxelHighlight;
    private VoxelController _voxelController;
    private Crosshair _crosshair;
    private GameSettings _gameSettings;
    private VoxelDataManager _voxelDataManager;
    private VoxelTextureAtlas _voxelTextureAtlas;

    public Game1() : base("KekLib3D Sandbox", 1280, 720, true)
    {
        IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        _gameSettings = GameSettings.FromFile("settings.ini", Content);
        _voxelDataManager = new VoxelDataManager();
        _voxelDataManager.LoadFromXml(Content, "voxels.xml");

        var requiredTextures = _voxelDataManager.GetAllUniqueTextureNames();
        _voxelTextureAtlas = new VoxelTextureAtlas(GraphicsDevice, Content, requiredTextures);

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

        _camera = new ControllableFpsCamera(
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            new Vector3(0, 10, 0)
        )
        {
            Fov = _gameSettings.Fov,
            Speed = _gameSettings.MovingSpeed,
            MouseSensitivity = _gameSettings.MouseSensitivity,
            IsMouseGrabbed = true
        };

        _grid = new SandboxGrid(GraphicsDevice, 100, 100, 1f, Color.Gray);

        _voxelMap = new VoxelMap();

        _voxelRenderer = new VoxelRenderer(GraphicsDevice);
        _voxelHighlight = new VoxelHighlight(GraphicsDevice, BasicEffect);
        _voxelController = new VoxelController(Input, _voxelHighlight, _voxelMap, GraphicsDevice, _camera, _grid);
        _crosshair = new Crosshair(GraphicsDevice, SpriteBatch)
        {
            Size = _gameSettings.CrosshairSize,
            Thickness = _gameSettings.CrosshairThickness,
            Color = new Color(_gameSettings.CrosshairColor)
        };
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);

        _voxelController.Update(gameTime);

        if (_voxelMap.IsDirty)
        {
            _voxelRenderer.Build(_voxelMap, _voxelDataManager, _voxelTextureAtlas);
            _voxelMap.ClearDirty();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        BasicEffect.World = Matrix.Identity;
        BasicEffect.View = _camera.ViewMatrix;
        BasicEffect.Projection = _camera.ProjectionMatrix;

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
        _grid.Draw(BasicEffect);
        _voxelHighlight.Draw();

        _crosshair.Draw();

        base.Draw(gameTime);
    }
}
