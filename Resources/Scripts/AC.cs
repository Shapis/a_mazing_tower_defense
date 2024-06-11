using Godot;
using System;
using System.Collections.Generic;

public partial class AC : Node
{
    [Export]
    private PackedScene[] _packedScenes;

    [Export]
    private PackedScene[] _towers;

    [Export]
    private PackedScene[] _enemies;

    public sealed override void _Ready()
    {
        if (GD.Load<PackedScene>("res://AssortedCatalog.tscn").Instantiate() is not AC temp)
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
        Silver,
        Blue,
        Green,
        Red,
        Orange,
    }

    public Color GetColor(AC.ColorPalette colorPalette, float alpha = 255f)
    {
        switch (colorPalette)
        {
            case AC.ColorPalette.Silver:
                return new Color(192f / 255f, 192f / 255f, 192f / 255f, alpha / 255f);
            case ColorPalette.Blue:
                return new Color(33f / 255f, 108f / 255f, 255f / 255f, alpha / 255f);
            case ColorPalette.Green:
                return new Color(30f / 255f, 255f / 255f, 0.0f / 255f, alpha / 255f);
            case ColorPalette.Red:
                return new Color(255f / 255f, 32f / 255f, 49f / 255f, alpha / 255f);
            case ColorPalette.Orange:
                return new Color(255f / 255f, 106f / 255f, 106f / 255f, alpha / 255f);
            default:
                return new Color(0f / 255f, 0f / 255f, 0f / 255f, alpha / 255f);
        }
    }
}
