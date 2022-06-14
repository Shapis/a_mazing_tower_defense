using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public partial class GameScene : Node2D, IBuildModeEvents
{
    TileMap? _map;
    private bool _isBuildModeActive = false;
    private bool _isBuildValid = false;
    private Vector2 _buildLocation;
    private Vector2i _buildTile;
    private AC.TowerName? _buildType;
    private int _currentWave = 0;
    private int _enemiesInWave = 0;

    public event Action<object, AC.TowerName>? OnBuildModeStartedEvent;
    public event Func<object, AC.TowerName, Vector2i?>? OnBuildModeEndedEvent;
    public override void _Ready()
    {
        _map = GetNode<TileMap>("Map");
        GetNode<UI>("UI").OnPausePlayPressedEvent += OnPausePlayPressed;
        GetNode<UI>("UI").OnBuildBtnDown += OnBuildBtnDown;
        GetNode<UI>("UI").OnBuildBtnUp += OnBuildBtnUp;

    }

    private void OnBuildBtnUp(object sender, AC.TowerName towerName)
    {
        // VerifyAndBuild();
        OnBuildModeEnded(this, towerName);


    }

    private void OnBuildBtnDown(object sender, AC.TowerName towerName)
    {
        OnBuildModeStarted(this, towerName);
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

    // private void OnUiOnSpeedUpPressed()
    // {
    //     if (_isBuildModeActive)
    //     {
    //         CancelBuildMode();
    //     }
    //     if (Engine.TimeScale == 2f)
    //     {
    //         Engine.TimeScale = 1f;
    //     }
    //     else
    //     {
    //         Engine.TimeScale = 2f;
    //     }
    // }


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
            var newEnemy = GD.Load<PackedScene>("res://Scenes/Enemies/" + item.Item1 + ".tscn").Instantiate() as BaseEnemy;
            if (_map is null)
            {
                GD.Print(this, "Map is null");
                return;
            }
            _map.GetNode<Path2D>("Path2D").AddChild(newEnemy, true);
            await ToSignal(GetTree().CreateTimer(item.Item2), "timeout");
        }
    }

    #endregion

    #region Building Methods
    public void OnBuildModeStarted(object sender, AC.TowerName towerName)
    {
        _buildType = towerName;
        _isBuildModeActive = true;
        OnBuildModeStartedEvent?.Invoke(sender, towerName);
    }


    public Vector2i? OnBuildModeEnded(object sender, AC.TowerName towerName)
    {
        _isBuildModeActive = false;
        _isBuildValid = false;
        return OnBuildModeEndedEvent?.Invoke(this, towerName);
    }

    private void VerifyAndBuild()
    {
        if (_isBuildValid)
        {

            var newTower = GetNode<AC>("/root/AC").GetTower(AC.TowerName.MachineGun);
            if (newTower is null)
            {
                GD.PrintErr("Failed to load tower");
                return;
            }
            newTower.Position = _buildLocation;
            newTower.IsBuilt = true;
            newTower.Rotate(-Mathf.Pi / 2);
            _map!.AddChild(newTower, true);
            var ac = GetNode<AC>("/root/AC");
            _map!.SetCell(ac.GetMapLayer(AC.MapLayerName.Towers), _buildTile, 1, new Vector2i(0, 0));
        }
    }


    #endregion
}