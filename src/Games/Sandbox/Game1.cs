using KekLib3D;
using KekLib3D.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sandbox;

public class Game1 : Core3D
{
    private VertexPositionColor[] _triangleVertices;
    private VertexBuffer _vertexBuffer;
    private ControllableFpsCamera _camera;
    private Matrix _worldMatrix = Matrix.Identity;

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
            GameSettings.Fov,
            GameSettings.MovingSpeed,
            GameSettings.MouseSensitivity
        );

        _triangleVertices = new VertexPositionColor[3];
        _triangleVertices[0] = new(new Vector3(0, 20, 0), Color.Red);
        _triangleVertices[1] = new(new Vector3(-20, -20, 0), Color.Green);
        _triangleVertices[2] = new(new Vector3(20, -20, 0), Color.Blue);

        _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_triangleVertices);
    }



    protected override void Update(GameTime gameTime)
    {
        _camera.Update(gameTime, Input);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        BasicEffect.Projection = _camera.ProjectionMatrix;
        BasicEffect.View = _camera.ViewMatrix;
        BasicEffect.World = _worldMatrix;

        GraphicsDevice.Clear(Color.Black);
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);

        RasterizerState rasterizerState = new()
        {
            CullMode = CullMode.None
        };

        GraphicsDevice.RasterizerState = rasterizerState;

        foreach (var pass in BasicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
        }


        base.Draw(gameTime);
    }
}
