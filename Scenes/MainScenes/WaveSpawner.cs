using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class WaveSpawner : Node
{
    [Signal]
    internal event Action<object, int>? OnWaveEndedEvent;

    private Map? _map;
    private AC? _assortedCatalog;
    Random rng = new Random();

    public int CurrentWave { get; private set; } = 0;
    public int TotalEnemiesInWave { get; private set; } = 0;
    public int TotalEnemiesLeft { get; private set; } = 0;
    public bool IsWaveInProgress { get; private set; } = false;
    private int mobsThatGotThrough = 0;

    public sealed override void _PhysicsProcess(float delta)
    {
        GD.Print(delta);
    }

    public void StartNextWave(Map map, AC assortedCatalog)
    {
        IsWaveInProgress = true;
        _map = map;
        _assortedCatalog = assortedCatalog;
        CurrentWave++;
        var waveEnemyList = GenerateWave();
        SetWaveInfo(waveEnemyList);
        SpawnWave(waveEnemyList);
    }

    private List<BaseEnemy> GenerateWave()
    {
        List<BaseEnemy> enemies = new List<BaseEnemy>();
        var targetTotalHealthOfEnemies = (int)(20 + CurrentWave * 40f);
        int totalHealthOfEnemies = 0;

        while (totalHealthOfEnemies < targetTotalHealthOfEnemies)
        {
            var randomNumber = rng.Next(0, Enum.GetNames(typeof(AC.EnemyType)).Length - 1);
            var enemy = _assortedCatalog!.GetEnemy((AC.EnemyType)randomNumber);
            randomNumber = rng.Next(0, enemy.MaxPossibleRarity);
            if (enemy.Rarity <= randomNumber)
            {
                enemies.Add(enemy);
                totalHealthOfEnemies += (int)enemy.Health;
            }
        }

        return enemies;
    }

    private void SetWaveInfo(List<BaseEnemy> waveEnemyList)
    {
        TotalEnemiesInWave = waveEnemyList.Count;
        TotalEnemiesLeft = TotalEnemiesInWave;
        mobsThatGotThrough = 0;
    }

    // TODO: Figure out how to GetTree() from this class, even though it's not on the tree so I can await a signal instead of using Task.Delay();
    private async void SpawnWave(List<BaseEnemy> waveEnemyList)
    {
        foreach (var item in waveEnemyList)
        {
            item.OnEnemyDiedEvent += OnEnemyDied;
            item.OnTargetReachedEvent += OnTargetReached;
            _map!.GetChildren().OfType<Path2D>().First().AddChild(item, true);

            await ToSignal(_map.GetTree().CreateTimer(1f, false), "timeout");
        }
    }

    // TODO: possibly if these two happen at the same time the game will get stuck. And I think there's a mild child of it happening since these two methods call QueueFree, so there's a small delay between being called and freeing the node.
    private void OnEnemyDied(object sender)
    {
        var temp = sender as BaseEnemy;
        temp!.OnEnemyDiedEvent -= OnEnemyDied;
        EnemyExpired(0);
    }

    private void OnTargetReached(object sender)
    {
        var temp = sender as BaseEnemy;
        temp!.OnTargetReachedEvent -= OnTargetReached;
        EnemyExpired(1);
    }

    private void EnemyExpired(int damageTaken)
    {
        TotalEnemiesLeft--;
        mobsThatGotThrough += damageTaken;

        if (TotalEnemiesLeft == 0)
        {
            IsWaveInProgress = false;
            OnWaveEndedEvent?.Invoke(this, mobsThatGotThrough);
        }
    }
}
