using KekLib3D;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Components;

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

    public Game1() : base("KekLib3D Sandbox", 1280, 720, true)
    {
        IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        _gameSettings = GameSettings.FromFile("settings.ini", Content);
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
        _voxelRenderer = new VoxelRenderer(GraphicsDevice, _voxelMap);
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
        _voxelRenderer.RebuildIfDirty();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        BasicEffect.World = Matrix.Identity;
        BasicEffect.View = _camera.ViewMatrix;
        BasicEffect.Projection = _camera.ProjectionMatrix;

        BasicEffect.VertexColorEnabled = true;
        BasicEffect.LightingEnabled = false;
        BasicEffect.DiffuseColor = Color.Gray.ToVector3();
        _grid.Draw(BasicEffect);

        BasicEffect.LightingEnabled = true;
        BasicEffect.EnableDefaultLighting();
        BasicEffect.PreferPerPixelLighting = false;
        BasicEffect.VertexColorEnabled = true;
        BasicEffect.DiffuseColor = Vector3.One;

        BasicEffect.DirectionalLight0.Enabled = true;
        BasicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f));
        BasicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
        BasicEffect.DirectionalLight0.SpecularColor = Vector3.Zero;

        BasicEffect.AmbientLightColor = new Vector3(0.3f, 0.3f, 0.3f);
        _voxelRenderer.Draw(BasicEffect);

        BasicEffect.LightingEnabled = false;
        _voxelHighlight.Draw();

        _crosshair.Draw();

        base.Draw(gameTime);
    }
}
