using KekLib2D.Core.Collision;
using KekLib2D.Core.Graphics;
using KekLib2D.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    public Circle PlayerBounds { get; private set; }
    private const float _velocity = 200.0f;
    private Vector2 Scale { get; set; }
    private TextureAtlas Atlas { get; set; }
    private AnimatedSprite Sprite { get; set; }
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
        UpdatePlayerBounds();

    }

    public void Update(GameTime gameTime, InputManager input, Rectangle screenBounds)
    {
        CheckKeyboardInput(gameTime, input);
        UpdatePlayerBounds();
        UpdatePositionWithinBounds(screenBounds);
        UpdateAnimation();
        Sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle screenBounds)
    {
        Sprite.Draw(spriteBatch, _position);
    }

    private void CheckKeyboardInput(GameTime gameTime, InputManager input)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (input.Keyboard.IsKeyDown(Keys.W))
        {
            _position.Y -= _velocity * dt;
            _isMoving = true;
            Direction = PlayerDirection.Backward;
            _isFlippedHorizontally = false;

        }
        if (input.Keyboard.IsKeyDown(Keys.S))
        {
            _position.Y += _velocity * dt;
            _isMoving = true;
            Direction = PlayerDirection.Forward;
            _isFlippedHorizontally = false;

        }
        if (input.Keyboard.IsKeyDown(Keys.A))
        {
            _position.X -= _velocity * dt;
            _isMoving = true;
            Direction = PlayerDirection.Left;
            _isFlippedHorizontally = true;

        }
        if (input.Keyboard.IsKeyDown(Keys.D))
        {
            _position.X += _velocity * dt;
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

    private void UpdatePositionWithinBounds(Rectangle screenBounds)
    {
        Vector2 newPosition = _position;

        newPosition.X = MathHelper.Clamp(newPosition.X, screenBounds.Left, screenBounds.Right - Sprite.Width);
        newPosition.Y = MathHelper.Clamp(newPosition.Y, screenBounds.Top, screenBounds.Bottom - Sprite.Height);

        _position = newPosition;

    }

    private void UpdatePlayerBounds()
    {
        PlayerBounds = new Circle((int)(_position.X + (Sprite.Width * 0.5f)), (int)(_position.Y + (Sprite.Height * 0.5f)), (int)(Sprite.Width * 0.5f));
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
