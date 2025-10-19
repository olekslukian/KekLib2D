using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Graphics;

public class FpsCamera(float width, float height, Vector3 position)
{
    public Vector3 Position { get; set; } = position;
    public float Pitch { get; set; }
    public float Yaw { get; set; } = -90.0f;
    public float Fov { get; set; } = 45.0f;
    public float ScreenWidth { get; private set; } = width;
    public float ScreenHeight { get; private set; } = height;
    public Vector3 Up => up;
    public Vector3 Front => front;
    public Vector3 Right => right;
    private Vector3 up = Vector3.Up;
    private Vector3 front = -Vector3.UnitZ;
    private Vector3 right = Vector3.Right;

    public Matrix ViewMatrix => Matrix.CreateLookAt(Position, Position + front, up);

    public Matrix ProjectionMatrix => Matrix.CreatePerspectiveFieldOfView(
        MathHelper.ToRadians(Fov),
        ScreenWidth / ScreenHeight,
        0.1f,
        500f
    );

    public void Update(Vector3 position)
    {
        Position = position;
        UpdateVectors();
    }

    public void Draw(BasicEffect basicEffect)
    {
        basicEffect.View = ViewMatrix;
        basicEffect.Projection = ProjectionMatrix;
    }

    private void UpdateVectors()
    {
        Pitch = MathHelper.Clamp(Pitch, -89f, 89f);

        front.X = (float)(Math.Cos(MathHelper.ToRadians(Pitch)) * Math.Cos(MathHelper.ToRadians(Yaw)));
        front.Y = (float)Math.Sin(MathHelper.ToRadians(Pitch));
        front.Z = (float)(Math.Cos(MathHelper.ToRadians(Pitch)) * Math.Sin(MathHelper.ToRadians(Yaw)));
        front = Vector3.Normalize(front);

        right = Vector3.Normalize(Vector3.Cross(front, Vector3.Up));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }
}
