using System.Linq;
using KekLib3D;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandbox;

public class Game1 : Core3D
{
    private ControllableFpsCamera _camera;
    private Matrix _worldMatrix = Matrix.Identity;
    private Model _model;

    public Game1() : base("KekLib3D Sandbox", 1280, 720, false)
    {
        IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();

        _camera = new ControllableFpsCamera(
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            new Vector3(0, 0, 0),
            GameSettings
        );



        _model = Content.Load<Model>("Models/cottage_fbx");
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        foreach (ModelMesh mesh in _model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects.Cast<BasicEffect>())
            {
                effect.AmbientLightColor = new Vector3(1f, 0, 0);
                effect.World = _worldMatrix;
                effect.View = _camera.ViewMatrix;
                effect.Projection = _camera.ProjectionMatrix;
                effect.EnableDefaultLighting();
            }
            mesh.Draw();
        }

        base.Draw(gameTime);
    }
}
