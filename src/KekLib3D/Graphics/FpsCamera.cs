using System;
using Microsoft.Xna.Framework;

namespace KekLib3D.Graphics;

public class FpsCamera
{
    public Vector3 Position { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    protected readonly float _screenWidth;
    protected readonly float _screenHeight;
    protected Vector3 up = Vector3.Up;
    protected Vector3 front = -Vector3.UnitZ;
    protected Vector3 right = Vector3.Right;
    protected float fov = 60.0f;

    public FpsCamera(float width, float height, Vector3 position, float fov)
    {
        _screenWidth = width;
        _screenHeight = height;
        Position = position;
        Yaw = -90.0f;
        this.fov = fov;
    }

    public Matrix ViewMatrix => Matrix.CreateLookAt(Position, Position + front, up);

    public Matrix ProjectionMatrix => Matrix.CreatePerspectiveFieldOfView(
        MathHelper.ToRadians(fov),
        _screenWidth / _screenHeight,
        0.1f,
        1000f
    );

    public void Update()
    {
        UpdateVectors();
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
