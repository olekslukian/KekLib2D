using KekLib3D;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandbox.Rendering;
using KekLib3D.Voxels.Rendering;
using KekLib3D.Components;
using KekLib3D.Voxels;
using MonoGame.ImGuiNet;
using Sandbox.UI;
using KekLib3D.Base;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Sandbox.Map;

namespace Sandbox;

public class Game1 : Core3D
{
    private static ImGuiRenderer _imGuiRenderer;
    private FpsCamera _camera;
    private ControllablePlayerWithCamera _player;
    private SandboxGrid _grid;
    private VoxelMap _voxelMap;
    private VoxelRenderer _voxelRenderer;
    private VoxelHighlight _voxelHighlight;
    private VoxelController _voxelController;
    private Crosshair _crosshair;
    private GameSettings _gameSettings;
    private VoxelDataManager _voxelDataManager;
    private VoxelTextureAtlas _voxelTextureAtlas;
    private UIController _menuController;
    private VoxelSelector _voxelSelector;

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

        _menuController = new UIController(Input);

        _imGuiRenderer = new(this);
        _imGuiRenderer.RebuildFontAtlas();

        _voxelSelector = new VoxelSelector(_menuController, _voxelDataManager);

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
            AreControlsEnabled = false,
        };

        _player = new ControllablePlayerWithCamera(basePlayer, _camera, BasicEffect)
        {
            MouseSensitivity = _gameSettings.MouseSensitivity,
        };

        var mapData = VoxelMapSerializer.LoadMap("./src/Games/Sandbox/maps/map.json");

        _grid = new SandboxGrid(GraphicsDevice, mapData.MapSize.Width, mapData.MapSize.Height, 1f, Color.Gray);

        _voxelMap = new VoxelMap();
        foreach (var voxel in mapData.Voxels)
        {
            var localId = _voxelDataManager.GetVoxelIdByName(voxel.Id);
            _voxelMap.Set(voxel.Position.ToInt3(), localId);
        }

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
        _menuController.Update();

        CheckIfMenuOpen();

        _player.Update(gameTime);

        _voxelController.Update(gameTime, selectedVoxelId: _voxelSelector.SelectedVoxelId);

        if (_voxelMap.IsDirty)
        {
            _voxelRenderer.Build(_voxelMap, _voxelDataManager, _voxelTextureAtlas);
            _voxelMap.ClearDirty();
        }

        if (Input.Keyboard.IsKeyPressed(Keys.Enter))
        {
            VoxelMapSerializer.SaveMap("./src/Games/Sandbox/maps/map.json", _voxelMap, _voxelDataManager, new Vector2(100, 100));
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        BasicEffect.World = Matrix.Identity;
        _player.Draw(gameTime);

        BasicEffect.TextureEnabled = true;
        BasicEffect.VertexColorEnabled = false;
        // BasicEffect.EnableDefaultLighting();

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

        _imGuiRenderer.BeginLayout(gameTime);

        _voxelSelector.Draw();

        _imGuiRenderer.EndLayout();
    }

    private void CheckIfMenuOpen()
    {
        if (_menuController.IsMenuShown)
        {
            _player.IsMouseGrabbed = false;
            _player.AreControlsEnabled = false;
            _voxelController.IsEnabled = false;
            IsMouseVisible = true;
        }
        else
        {
            _voxelController.IsEnabled = true;
            _player.IsMouseGrabbed = true;
            _player.AreControlsEnabled = true;
            IsMouseVisible = false;
        }
    }
}
