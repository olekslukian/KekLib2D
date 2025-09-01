using System;
using KekLib2D.Core.Audio;
using KekLib2D.Core.Collision;
using KekLib2D.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SlimeKiller.Entities;

public class Slime
{
    private const float MOVEMENT_SPEED = 200.0f;
    public Circle Bounds { get; private set; }
    public AnimatedSprite Sprite { get; private set; }
    public bool IsMoving { get; private set; }
    public event Action OnBounce;
    public event Action OnCollected;
    private ContentManager Content { get; set; }
    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 Scale { get; set; }
    private TextureAtlas Atlas { get; set; }
    private bool _isFlippedHorizontally = false;
    private string _currentAnimationName;


    public Slime(Vector2 initialPosition, ContentManager content)
    {
        Content = content;
        Atlas = TextureAtlas.FromFile(Content, "Sprites/slime-atlas-definitions.xml");
        Scale = new Vector2(4.0f, 4.0f);
        _position = initialPosition;
        UpdateAnimation();
        SetSlimeBounds();
        AssignRandomVelocity();
    }

    public void Update(GameTime gameTime)
    {
        UpdateAnimation();
        Sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, _position);
    }

    public void OnCollision(PresentationParameters presentationParams)
    {
        AssignRandomPosition(presentationParams);
        AssignRandomVelocity();
        OnCollected?.Invoke();
    }

    private void AssignRandomVelocity()
    {
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new(x, y);

        _velocity = direction * MOVEMENT_SPEED;
    }

    private void AssignRandomPosition(PresentationParameters presentationParams)
    {
        Random random = new();
        int totalColumns = presentationParams.BackBufferWidth / (int)Sprite.Width;
        int totalRows = presentationParams.BackBufferHeight / (int)Sprite.Height;
        int column = random.Next(0, totalColumns);
        int row = random.Next(0, totalRows);
        _position = new Vector2(column * Sprite.Width, row * Sprite.Height);
    }


    public void CheckIfInRoomBounds(GameTime gameTime, Rectangle roomBounds)
    {
        SetSlimeBounds();


        Vector2 normal = Vector2.Zero;

        if (Bounds.Left < roomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            _position.X = roomBounds.Left;
        }
        else if (Bounds.Right > roomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            _position.X = roomBounds.Right - Sprite.Width;
        }

        if (Bounds.Top < roomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            _position.Y = roomBounds.Top;
        }
        else if (Bounds.Bottom > roomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            _position.Y = roomBounds.Bottom - Sprite.Height;
        }

        if (normal != Vector2.Zero)
        {
            _velocity = Vector2.Reflect(_velocity, normal);
            OnBounce?.Invoke();
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
        IsMoving = _velocity != Vector2.Zero;
        _isFlippedHorizontally = _velocity.X < 0;

        if (IsMoving)
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
        Sprite.LayerDepth = 1.0f;
    }
}
