using System;
using Microsoft.Xna.Framework;

namespace KekLib2D.Core.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public AnimatedSprite() { }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed >= Animation.Delay)
        {
            _elapsed -= Animation.Delay;
            _currentFrame++;

            if (_currentFrame >= Animation.Frames.Count)
            {
                _currentFrame = 0;
            }

            Region = Animation.Frames[_currentFrame];
        }
    }
}
