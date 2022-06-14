using Godot;
using System;
using System.Collections.Generic;

public partial class AC : Node
{
    [Export] private List<PackedScene>? _packedScenes;
    [Export] private List<PackedScene>? _towers;

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

        temp.Dispose();

    }

    public enum SceneName
    {
        GameScene,

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

    public enum MapLayerName
    {
        Ground,
        Roads,
        Props,
        TowerPreviews,
        Towers,
    }

    public int GetMapLayer(AC.MapLayerName mapLayerName)
    {
        return (int)mapLayerName;
    }
}



