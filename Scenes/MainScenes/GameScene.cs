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
    private int _currentWave = 0;
    private int _enemiesInWave = 0;

    // public event Action<object, AC.TowerType>? OnBuildModeStartedEvent;
    // public event Func<object, AC.TowerType, Vector2i?>? OnBuildModeEndedEvent;
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

        _bottomBar.OnPausePlayPressedEvent += OnPausePlayPressed;
        _bottomBar.OnBuildBtnDown += OnBuildBtnDown;
        _bottomBar.OnBuildBtnUp += OnBuildBtnUp;
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

    public sealed override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("ui_accept"))
        {
            _bottomBar!.AddTower(AC.TowerType.GunTurret);
        }
    }

    private bool OnPausePlayPressed()
    {
        if (_isBuildModeActive)
        {
            // CancelBuildMode();
        }
        if (_currentWave == 0)
        {
            StartNextWave();
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Wave Methods

    private List<Tuple<string, float>> RetrieveWaveData()
    {
        var waveData = new List<Tuple<string, float>>();
        waveData.Add(new Tuple<string, float>("BlueTank", 1.7f));
        waveData.Add(new Tuple<string, float>("BlueTank", 0.1f));
        _enemiesInWave = waveData.Count;
        return waveData;
    }

    private async void StartNextWave()
    {
        _currentWave++;
        var waveData = RetrieveWaveData();
        await ToSignal(GetTree().CreateTimer(0.2), "timeout");
        SpawnEnemies(waveData);
    }

    private async void SpawnEnemies(List<Tuple<string, float>> waveData)
    {
        foreach (var item in waveData)
        {
            var newEnemy =
                GD.Load<PackedScene>("res://Scenes/Enemies/" + item.Item1 + ".tscn").Instantiate()
                as BaseEnemy;
            if (_map is null)
            {
                GD.Print(this, "Map is null");
                return;
            }

            _map.GetChildren().OfType<Path2D>().First().AddChild(newEnemy, true);
            await ToSignal(GetTree().CreateTimer(item.Item2), "timeout");
        }
    }

    #endregion
}
