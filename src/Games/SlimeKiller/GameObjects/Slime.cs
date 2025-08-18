using System;
using KekLib2D.Core.Collision;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeKiller.GameObjects;

public class Slime
{
    private const float MOVEMENT_SPEED = 200.0f;
    public Circle Bounds { get; private set; }
    public AnimatedSprite Sprite { get; private set; }
    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 Scale { get; set; }
    private TextureAtlas Atlas { get; set; }
    private bool _isMoving = false;
    private bool _isFlippedHorizontally = false;
    private string _currentAnimationName;


    public Slime(TextureAtlas atlas, Vector2 initialPosition)
    {
        Atlas = atlas;
        Scale = new Vector2(4.0f, 4.0f);
        _position = initialPosition;
        AssignRandomVelocity();
        UpdateAnimation();
    }

    public void Update(GameTime gameTime, Rectangle screenBounds)
    {
        CheckIfInScreenBounds(screenBounds, gameTime);
        UpdateAnimation();
        Sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, _position);
    }

    public void OnCollision(Vector2 newPos)
    {
        _position = newPos;
        AssignRandomVelocity();
    }

    private void AssignRandomVelocity()
    {
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new(x, y);

        _velocity = direction * MOVEMENT_SPEED;
    }


    private void CheckIfInScreenBounds(Rectangle screenBounds, GameTime gameTime)
    {
        SetSlimeBounds();

        Vector2 normal = Vector2.Zero;

        if (Bounds.Left < screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            _position.X = screenBounds.Left;
        }
        else if (Bounds.Right > screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            _position.X = screenBounds.Right - Sprite.Width;
        }

        if (Bounds.Top < screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            _position.Y = screenBounds.Top;
        }
        else if (Bounds.Bottom > screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            _position.Y = screenBounds.Bottom - Sprite.Height;
        }

        if (normal != Vector2.Zero)
        {
            _velocity = Vector2.Reflect(_velocity, normal);
        }

        UpdatePosition(gameTime);
    }

    private void SetSlimeBounds()
    {
        Bounds = new Circle((int)(_position.X + (Sprite.Width * 0.5f)), (int)(_position.Y + (Sprite.Height * 0.5f)), (int)(Sprite.Width * 0.5f));
    }

    private void UpdatePosition(GameTime gameTime)
    {
        _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

    }


    private void UpdateAnimation()
    {
        if (_isMoving)
        {
            if (_currentAnimationName != "slime-walk")
            {
                _currentAnimationName = "slime-walk";
                Sprite = Atlas.CreateAnimatedSprite(_currentAnimationName);
                Sprite.Scale = Scale;
            }
        }
        else
        {
            if (_currentAnimationName != "slime-idle")
            {
                _currentAnimationName = "slime-idle";
                Sprite = Atlas.CreateAnimatedSprite(_currentAnimationName);
                Sprite.Scale = Scale;
            }
        }

        Sprite.Effects = _isFlippedHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }
}
