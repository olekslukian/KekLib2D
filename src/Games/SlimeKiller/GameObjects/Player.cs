using System;
using KekLib2D.Core.Collision;
using KekLib2D.Core.Graphics;
using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SlimeKiller.GameObjects;

namespace SlimeKiller;

public enum PlayerDirection
{
    Forward,
    Backward,
    Left,
    Right,
}

public class Player
{
    private const float MOVEMENT_SPEED = 200.0f;
    public Circle Bounds { get; private set; }
    public AnimatedSprite Sprite { get; private set; }
    private Vector2 Scale { get; set; }
    private TextureAtlas Atlas { get; set; }
    private PlayerDirection Direction { get; set; }
    private Vector2 _position;
    private bool _isMoving = false;
    private bool _isFlippedHorizontally = false;
    private string _currentAnimationName;

    public Player(TextureAtlas atlas, Vector2 initialPosition, PlayerDirection initialDirection)
    {
        Atlas = atlas;
        Scale = new Vector2(4.0f, 4.0f);
        _position = initialPosition;
        Direction = initialDirection;
        UpdateAnimation();
    }

    public void Update(GameTime gameTime, InputManager input, Rectangle screenBounds, Slime slime, GraphicsDevice graphicsDevice)
    {
        CheckKeyboardInput(gameTime, input);
        CheckIfInScreenBounds(screenBounds);
        CheckEnemyCollision(slime, graphicsDevice);
        UpdateAnimation();
        Sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite.Draw(spriteBatch, _position);
    }

    private void CheckKeyboardInput(GameTime gameTime, InputManager input)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (input.Keyboard.IsKeyDown(Keys.W))
        {
            _position.Y -= MOVEMENT_SPEED * dt;
            _isMoving = true;
            Direction = PlayerDirection.Backward;
            _isFlippedHorizontally = false;

        }
        if (input.Keyboard.IsKeyDown(Keys.S))
        {
            _position.Y += MOVEMENT_SPEED * dt;
            _isMoving = true;
            Direction = PlayerDirection.Forward;
            _isFlippedHorizontally = false;

        }
        if (input.Keyboard.IsKeyDown(Keys.A))
        {
            _position.X -= MOVEMENT_SPEED * dt;
            _isMoving = true;
            Direction = PlayerDirection.Left;
            _isFlippedHorizontally = true;

        }
        if (input.Keyboard.IsKeyDown(Keys.D))
        {
            _position.X += MOVEMENT_SPEED * dt;
            _isMoving = true;
            Direction = PlayerDirection.Right;
            _isFlippedHorizontally = false;

        }

        if (input.Keyboard.IsKeyUp(Keys.W) && input.Keyboard.IsKeyUp(Keys.S) &&
            input.Keyboard.IsKeyUp(Keys.A) && input.Keyboard.IsKeyUp(Keys.D))
        {
            _isMoving = false;
        }
    }

    private void CheckIfInScreenBounds(Rectangle screenBounds)
    {
        SetPlayerBounds();

        if (Bounds.Left < screenBounds.Left)
        {
            _position.X = screenBounds.Left;
        }
        else if (Bounds.Right > screenBounds.Right)
        {
            _position.X = screenBounds.Right - Sprite.Width;
        }

        if (Bounds.Top < screenBounds.Top)
        {
            _position.Y = screenBounds.Top;
        }
        else if (Bounds.Bottom > screenBounds.Bottom)
        {
            _position.Y = screenBounds.Bottom - Sprite.Height;
        }
    }

    private void CheckEnemyCollision(Slime slime, GraphicsDevice graphicsDevice)
    {
        if (Bounds.Intersects(slime.Bounds))
        {
            int totalColumns = graphicsDevice.PresentationParameters.BackBufferWidth / (int)slime.Sprite.Width;
            int totalRows = graphicsDevice.PresentationParameters.BackBufferHeight / (int)slime.Sprite.Height;

            int column = Random.Shared.Next(0, totalColumns);
            int row = Random.Shared.Next(0, totalRows);

            slime.OnCollision(new Vector2(column * slime.Sprite.Width, row * slime.Sprite.Height));
        }
    }

    private void SetPlayerBounds()
    {
        Bounds = new Circle((int)(_position.X + (Sprite.Width * 0.5f)), (int)(_position.Y + (Sprite.Height * 0.5f)), (int)(Sprite.Width * 0.5f));
    }

    private void UpdateAnimation()
    {
        string animationName = Direction switch
        {
            PlayerDirection.Forward => _isMoving ? "player-walk-forward" : "player-idle-forward",
            PlayerDirection.Backward => _isMoving ? "player-walk-back" : "player-idle-back",
            PlayerDirection.Left or PlayerDirection.Right => _isMoving ? "player-walk-right" : "player-idle-right",
            _ => _isMoving ? "player-walk-forward" : "player-idle-forward"
        };

        if (_currentAnimationName != animationName)
        {
            Sprite = Atlas.CreateAnimatedSprite(animationName);
            Sprite.Scale = Scale;
            _currentAnimationName = animationName;
        }
        Sprite.Effects = _isFlippedHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Sprite.LayerDepth = 1.0f;
    }
}
