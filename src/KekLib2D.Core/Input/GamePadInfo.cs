
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KekLib2D.Core.Input;

public class GamePadInfo
{
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;
    public PlayerIndex PlayerIndex { get; }
    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; }
    public bool IsConnected => CurrentState.IsConnected;
    public Vector2 LeftThumbstick => CurrentState.ThumbSticks
    .Left;
    public Vector2 RightThumbstick => CurrentState.ThumbSticks.Right;
    public float LeftTrigger => CurrentState.Triggers.Left;
    public float RightTrigger => CurrentState.Triggers.Right;

    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = GamePad.GetState(playerIndex);
        CurrentState = PreviousState;
    }

    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

            if (_vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
            }
        }
    }

    public bool IsButtonDown(Buttons button) => CurrentState.IsButtonDown(button);

    public bool IsButtonUp(Buttons button) => CurrentState.IsButtonUp(button);

    public bool IsButtonPressed(Buttons button) => CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);

    public bool IsButtonReleased(Buttons button) => CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);

    public void SetVibration(float strength, TimeSpan time)
    {
        _vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }
}
