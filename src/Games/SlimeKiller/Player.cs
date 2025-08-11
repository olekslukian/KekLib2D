using System;
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
    private const float _velocity = 200.0f;
    private Vector2 Scale { get; set; }
    private TextureAtlas Atlas { get; set; }
    private AnimatedSprite Sprite { get; set; }
    private PlayerDirection Direction { get; set; }
    private Vector2 _position;
    private bool _isMoving = false;
    private bool _isFlippedHorizontally = false;

    public Player(TextureAtlas atlas, Vector2 initialPosition, PlayerDirection initialDirection)
    {
        Atlas = atlas;
        Scale = new Vector2(4.0f, 4.0f);
        _position = initialPosition;
        Direction = initialDirection;
        UpdateAnimation();
    }

    public void Update(GameTime gameTime, InputManager input)
    {
        CheckKeyboardInput(gameTime, input);
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

        if (input.Keyboard.IsKeyPressed(Keys.W))
        {
            Direction = PlayerDirection.Backward;
            _isFlippedHorizontally = false;
        }
        if (input.Keyboard.IsKeyPressed(Keys.S))
        {
            Direction = PlayerDirection.Forward;
            _isFlippedHorizontally = false;
        }
        if (input.Keyboard.IsKeyPressed(Keys.A))
        {
            Direction = PlayerDirection.Left;
            _isFlippedHorizontally = true;
        }
        if (input.Keyboard.IsKeyPressed(Keys.D))
        {
            Direction = PlayerDirection.Right;
            _isFlippedHorizontally = false;
        }

        if (input.Keyboard.IsKeyDown(Keys.W))
        {
            _position.Y -= _velocity * dt;
            _isMoving = true;
        }
        if (input.Keyboard.IsKeyDown(Keys.S))
        {
            _position.Y += _velocity * dt;
            _isMoving = true;
        }
        if (input.Keyboard.IsKeyDown(Keys.A))
        {
            _position.X -= _velocity * dt;
            _isMoving = true;
        }
        if (input.Keyboard.IsKeyDown(Keys.D))
        {
            _position.X += _velocity * dt;
            _isMoving = true;
        }

        if (input.Keyboard.IsKeyUp(Keys.W) && input.Keyboard.IsKeyUp(Keys.S) &&
            input.Keyboard.IsKeyUp(Keys.A) && input.Keyboard.IsKeyUp(Keys.D))
        {
            _isMoving = false;
        }
    }

    private void UpdateAnimation()
    {
        switch (Direction)
        {
            case PlayerDirection.Forward:
                SelectSprite(_isMoving ? Atlas.CreateAnimatedSprite("player-idle-forward") : Atlas.CreateAnimatedSprite("player-walk-forward"));
                break;
            case PlayerDirection.Backward:
                SelectSprite(_isMoving ? Atlas.CreateAnimatedSprite("player-walk-back") : Atlas.CreateAnimatedSprite("player-idle-back"));
                break;
            case PlayerDirection.Left:
            case PlayerDirection.Right:
                SelectSprite(_isMoving ? Atlas.CreateAnimatedSprite("player-walk-right") : Atlas.CreateAnimatedSprite("player-idle-right"));
                break;
            default:
                SelectSprite(_isMoving ? Atlas.CreateAnimatedSprite("player-idle-back") : Atlas.CreateAnimatedSprite("player-walk-back"));
                break;
        }
    }

    private void SelectSprite(AnimatedSprite newSprite)
    {
        if (newSprite != Sprite)
        {
            Sprite = newSprite;
            Sprite.Scale = Scale;
            Sprite.Effects = _isFlippedHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Sprite.LayerDepth = 1f;
        }
    }
}
