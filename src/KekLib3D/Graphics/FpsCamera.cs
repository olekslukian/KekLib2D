using System;
using Microsoft.Xna.Framework;

namespace KekLib3D.Graphics;

public class FpsCamera(float width, float height, Vector3 position)
{
    public Vector3 Position { get; set; } = position;
    public float Pitch { get; set; }
    public float Yaw { get; set; } = -90.0f;
    public float Fov { get; set; } = 45.0f;
    public Vector3 Up => up;
    public Vector3 Front => front;
    public Vector3 Right => right;
    private Vector3 up = Vector3.Up;
    private Vector3 front = -Vector3.UnitZ;
    private Vector3 right = Vector3.Right;
    protected readonly float _screenWidth = width;
    protected readonly float _screenHeight = height;

    public Matrix ViewMatrix => Matrix.CreateLookAt(Position, Position + front, up);

    public Matrix ProjectionMatrix => Matrix.CreatePerspectiveFieldOfView(
        MathHelper.ToRadians(Fov),
        _screenWidth / _screenHeight,
        0.1f,
        500f
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
