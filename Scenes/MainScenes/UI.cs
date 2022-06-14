using Godot;
using System;

public partial class UI : CanvasLayer
{
    [Signal] public event Func<bool>? OnPausePlayPressedEvent;
    [Signal] public event Action<object, AC.TowerName>? OnBuildBtnDown;
    [Signal] public event Action<object, AC.TowerName>? OnBuildBtnUp;



    private void OnPausePlayPressed()
    {
        if (OnPausePlayPressedEvent?.Invoke() == true)
        {
            return;
        }
        GetTree().Paused = !GetTree().Paused;
    }

    #region  build button events
    private void OnBuildBtn0Down()
    {
        OnBuildBtnDown?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn1Down()
    {
        OnBuildBtnDown?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn2Down()
    {
        OnBuildBtnDown?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn3Down()
    {
        OnBuildBtnDown?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn0Up()
    {
        OnBuildBtnUp?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn1Up()
    {
        OnBuildBtnUp?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn2Up()
    {
        OnBuildBtnUp?.Invoke(this, AC.TowerName.MachineGun);
    }

    private void OnBuildBtn3Up()
    {
        OnBuildBtnUp?.Invoke(this, AC.TowerName.MachineGun);
    }


    #endregion

}
