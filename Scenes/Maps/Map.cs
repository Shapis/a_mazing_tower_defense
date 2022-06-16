using Godot;
using System;

public partial class Map : TileMap
{
    [Export]
    private NodePath? _towerContainerPath;
    private Node? _towerContainer;

    [Export]
    private NodePath? _enemyPath2DPath;
    private EnemyPath2D? _enemyPath2D;

    public sealed override void _Ready()
    {
        _towerContainer = GetNode<Node>(_towerContainerPath);
        _enemyPath2D = GetNode<EnemyPath2D>(_enemyPath2DPath);
    }

    internal bool VerifyAndBuildTower(AC.TowerType towerType)
    {
        var buildTile = VerifyBuildLocation().Item1;
        var isBlocked = VerifyBuildLocation().Item2;

        if (buildTile is null || isBlocked)
        {
            return false;
        }

        var nullSafeBuildTile = (Vector2i)buildTile;

        var tup2 = VerifyBuildLocation().Item2;

        var newTower = GetNode<AC>("/root/AC").GetTower(towerType);
        if (newTower is null)
        {
            GD.PrintErr("Failed to load tower");
            return false;
        }
        newTower.Position = MapToWorld(nullSafeBuildTile);
        newTower.IsBuilt = true;
        newTower.Rotate(-Mathf.Pi / 2);
        _towerContainer!.AddChild(newTower, true);
        var ac = GetNode<AC>("/root/AC");
        SetCell(ac.GetMapLayer(AC.MapLayerName.Towers), nullSafeBuildTile, 1, new Vector2i(0, 0));
        return true;
    }

    // The Vector2i returns null if outside the map, or the tile index if in the map, and the bool returns whether the tile is blocked or not.
    internal Tuple<Vector2i?, bool> VerifyBuildLocation()
    {
        Vector2 mousePosition = GetGlobalMousePosition();

        var currentTile = WorldToMap(mousePosition);
        var tilePosition = MapToWorld(currentTile);

        bool doesCellExist = false;
        for (int i = 0; i < 4; i++)
        {
            if (GetCellSourceId(i, currentTile, true) != -1)
            {
                doesCellExist = true;
                break;
            }
        }
        bool isCellBlocked = false;
        for (int i = 1; i <= 4; i++)
        {
            if (GetCellSourceId(i, currentTile, true) != -1)
            {
                isCellBlocked = true;
            }
        }

        SetCell((int)AC.MapLayerName.TowerPreviews, currentTile, 1, new Vector2i(0, 0));

        // If the path couldnt be generated return false and dont build the path2d.
        var isReachable = _enemyPath2D!.GeneratePath();
        _enemyPath2D.BuildPath2D();

        ClearLayer((int)AC.MapLayerName.TowerPreviews);

        if (!isCellBlocked && doesCellExist && isReachable)
        {
            // UpdateTowerPreview(tilePosition, "1eff0096");
            return new Tuple<Vector2i?, bool>(currentTile, false);
        }
        else if (!doesCellExist)
        {
            // UpdateTowerPreview(mousePosition, "ff2031b8");
            return new Tuple<Vector2i?, bool>(null, true);
        }
        else
        {
            // UpdateTowerPreview(tilePosition, "ff2031b8");
            return new Tuple<Vector2i?, bool>(currentTile, true);
        }
    }
}
