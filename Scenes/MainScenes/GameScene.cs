using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class GameScene : Node2D
{
    [Export]
    private NodePath? _bottomBarPath;
    private BottomBar? _bottomBar;

    [Export]
    private NodePath? _mapPath;
    private Map? _map;

    [Export]
    private NodePath? _towerPreviewPath;
    private TowerPreview? _towerPreview;

    private bool _isBuildModeActive = false;

    private WaveSpawner? _waveSpawner;

    public override void _Ready()
    {
        _bottomBar = GetNode<BottomBar>(_bottomBarPath);
        _map = GetNode<Map>(_mapPath);
        _towerPreview = GetNode<TowerPreview>(_towerPreviewPath);

        if (_bottomBar is null)
        {
            GD.PrintErr("BottomBar is null");
            return;
        }

        if (_map is null)
        {
            GD.PrintErr("Map is null");
            return;
        }

        if (_towerPreview is null)
        {
            GD.PrintErr("TowerPreview is null");
            return;
        }

        _waveSpawner = new WaveSpawner();
        _waveSpawner.OnWaveEndedEvent += OnWaveEnded;

        _bottomBar.OnPausePlayPressedEvent += OnPausePlayPressed;
        _bottomBar.OnBuildBtnDown += OnBuildBtnDown;
        _bottomBar.OnBuildBtnUp += OnBuildBtnUp;
    }

    private void OnWaveEnded(object sender, int mobsThatGotThrough)
    {
        _bottomBar!.ResetPausePlayBtn();
    }

    private bool OnBuildBtnUp(object sender, AC.TowerType towerType)
    {
        _towerPreview!.EndBuildModePreview();

        return _map!.VerifyAndBuildTower(towerType);
    }

    private void OnBuildBtnDown(object sender, AC.TowerType towerName)
    {
        _towerPreview!.InitiateBuildModePreview(towerName, _map);
    }

    private BaseTower? _currentTower;

    public sealed override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("ui_cancel"))
        {
            _bottomBar!.AddTower(AC.TowerType.GunTurret);
            // GetViewport().SetInputAsHandled();
        }
        if (inputEvent.IsActionPressed("ui_accept"))
        {
            _currentTower = _map!.GetTowerAt(GetGlobalMousePosition());

            if (_currentTower is not null)
            {
                _towerPreview!.InitiateBuildModePreview(_currentTower.TowerType, _map);
                _map!.RemoveTowerAt(GetGlobalMousePosition());
            }
        }

        if (inputEvent.IsActionReleased("ui_accept"))
        {
            if (_currentTower is not null)
            {
                _towerPreview!.EndBuildModePreview();
                if (!_map!.VerifyAndBuildTower(_currentTower.TowerType))
                {
                    _bottomBar!.AddTower(_currentTower.TowerType, true);
                }

                _currentTower = null;
            }
        }
    }

    private bool OnPausePlayPressed(object sender)
    {
        if (_isBuildModeActive)
        {
            _towerPreview!.EndBuildModePreview();
        }
        if (!_waveSpawner!.IsWaveInProgress)
        {
            var ac = GetNode<AC>("/root/AC");
            _waveSpawner.StartNextWave(_map!, ac);
            return true;
        }
        else
        {
            return false;
        }
    }
}
