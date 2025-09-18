using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KekLib3D.Graphics;

public static class Raycaster
{
  public static Ray CastRay(GraphicsDevice graphicsDevice, FpsCamera camera)
  {
    Vector3 nearPoint = new(graphicsDevice.PresentationParameters.BackBufferWidth / 2, graphicsDevice.PresentationParameters.BackBufferHeight / 2, 0);
    Vector3 farPoint = new(graphicsDevice.PresentationParameters.BackBufferWidth / 2, graphicsDevice.PresentationParameters.BackBufferHeight / 2, 1);

    Vector3 nearWorld = graphicsDevice.Viewport.Unproject(nearPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
    Vector3 farWorld = graphicsDevice.Viewport.Unproject(farPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

    Vector3 direction = Vector3.Normalize(farWorld - nearWorld);
    return new Ray(nearWorld, direction);
  }

}
