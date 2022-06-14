using Godot;
using System;

public partial class UI : CanvasLayer
{
    [Signal] public event Func<bool>? OnPausePlayPressedEvent;
    [Signal] public event Action? OnSpeedUpPressedEvent;



    #region Game Control Methods
    private void OnPausePlayPressed()
    {
        if (OnPausePlayPressedEvent?.Invoke() == true)
        {
            return;
        }
        GetTree().Paused = !GetTree().Paused;
    }

    private void OnSpeedUpPressed()
    {
        EmitSignal(nameof(OnSpeedUpPressedEvent));
    }
    #endregion

}
