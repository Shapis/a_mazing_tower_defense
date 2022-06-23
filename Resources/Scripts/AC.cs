using Godot;
using System;
using System.Collections.Generic;

public partial class AC : Node
{
    [Export]
    private List<PackedScene>? _packedScenes;

    [Export]
    private List<PackedScene>? _towers;

    [Export]
    private List<PackedScene>? _enemies;

    public sealed override void _Ready()
    {
        AC? temp = GD.Load<PackedScene>("res://AssortedCatalog.tscn").Instantiate() as AC;

        if (temp is null)
        {
            GD.PrintErr("Failed to load AssortedCatalog.tscn");
            return;
        }

        _packedScenes = temp._packedScenes;
        _towers = temp._towers;
        _enemies = temp._enemies;

        temp.Dispose();
    }

    public enum SceneName
    {
        GameScene,
        MainMenu,
    }

    public PackedScene GetPackedScene(AC.SceneName sceneName)
    {
        return _packedScenes![(int)sceneName];
    }

    public enum TowerType
    {
        GunTurret,
        DualGunTurret,
    }

    public BaseTower GetTower(AC.TowerType towerName)
    {
        return _towers![(int)towerName].Instantiate<BaseTower>();
    }

    public enum EnemyType
    {
        BlueTank,
    }

    public BaseEnemy GetEnemy(AC.EnemyType enemyName)
    {
        return _enemies![(int)enemyName].Instantiate<BaseEnemy>();
    }

    public enum MapLayerName
    {
        Ground,
        Roads,
        Props,
        TowerPreviews,
        Towers,
    }

    public enum ColorPalette
    {
        Blue,
        Green,
        Red,
        Orange,
    }

    public Color GetColor(AC.ColorPalette colorPalette, int alpha = 1)
    {
        switch (colorPalette)
        {
            case ColorPalette.Blue:
                return new Color(0.0f, 0.0f, 1.0f, alpha);
            case ColorPalette.Green:
                return new Color(30f, 255f, 0.0f, 150f);
            default:
                return new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }
}
