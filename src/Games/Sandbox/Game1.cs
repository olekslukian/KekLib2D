using KekLib3D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Sandbox;

public class Game1 : Core3D
{
    private Vector3 _cameraTarget;
    private Vector3 _cameraPosition;
    private Matrix _viewMatrix;
    private Matrix _projectionMatrix;
    private Matrix _worldMatrix;
    private VertexPositionColor[] _triangleVertices;
    private VertexBuffer _vertexBuffer;
    private bool _orbit = false;
    private float _fov = 45f;

    public Game1() : base("KekLib3D Sandbox", 1280, 720, false)
    {
    }

    protected override void LoadContent()
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        _cameraTarget = Vector3.Zero;
        _cameraPosition = new Vector3(0f, 0f, -100f);
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(_fov),
            GraphicsDevice.Viewport.AspectRatio,
            1f,
            1000f
        );
        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.Up);
        _worldMatrix = Matrix.CreateWorld(_cameraTarget, Vector3.Forward, Vector3.Up);

        _triangleVertices = new VertexPositionColor[3];
        _triangleVertices[0] = new(new Vector3(0, 20, 0), Color.Red);
        _triangleVertices[1] = new(new Vector3(-20, -20, 0), Color.Green);
        _triangleVertices[2] = new(new Vector3(20, -20, 0), Color.Blue);

        _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        _vertexBuffer.SetData(_triangleVertices);
    }



    protected override void Update(GameTime gameTime)
    {
        if (Input.Keyboard.IsKeyDown(Keys.A))
        {
            _cameraPosition.X -= 1f;
            _cameraTarget.X -= 1f;
        }
        if (Input.Keyboard.IsKeyDown(Keys.D))
        {
            _cameraPosition.X += 1f;
            _cameraTarget.X += 1f;
        }
        if (Input.Keyboard.IsKeyDown(Keys.W))
        {
            _cameraPosition.Y -= 1f;
            _cameraTarget.Y -= 1f;
        }
        if (Input.Keyboard.IsKeyDown(Keys.S))
        {
            _cameraPosition.Y += 1f;
            _cameraTarget.Y += 1f;
        }
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            _cameraPosition.Z += 1f;
        }
        if (Input.Keyboard.IsKeyDown(Keys.LeftShift))
        {
            _cameraPosition.Z -= 1f;
        }
        if (Input.Keyboard.IsKeyPressed(Keys.O))
        {
            _orbit = !_orbit;
        }

        if (_orbit)
        {
            Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            _cameraPosition = Vector3.Transform(_cameraPosition, rotation);
        }

        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.Up);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        BasicEffect.Projection = _projectionMatrix;
        BasicEffect.View = _viewMatrix;
        BasicEffect.World = _worldMatrix;

        GraphicsDevice.Clear(Color.CornflowerBlue);
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
