using Godot;
using System;

public partial class Map : TileMap
{
    internal void BuildTower(Vector2i buildTile, AC.TowerType towerType)
    {
        var newTower = GetNode<AC>("/root/AC").GetTower(towerType);
        if (newTower is null)
        {
            GD.PrintErr("Failed to load tower");
            return;
        }
        newTower.Position = MapToWorld(buildTile);
        newTower.IsBuilt = true;
        newTower.Rotate(-Mathf.Pi / 2);
        AddChild(newTower, true);
        var ac = GetNode<AC>("/root/AC");
        SetCell(ac.GetMapLayer(AC.MapLayerName.Towers), buildTile, 1, new Vector2i(0, 0));
    }

    // The Vector2i returns null if outside the map, or the tile index if in the map, and the bool returns whether the tile is blocked or not.
    internal Tuple<Vector2?, bool> VerifyBuildLocation()
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

        if (!isCellBlocked && doesCellExist)
        {
            // UpdateTowerPreview(tilePosition, "1eff0096");
            return new Tuple<Vector2?, bool>(tilePosition, false);
        }
        else if (!doesCellExist)
        {
            // UpdateTowerPreview(mousePosition, "ff2031b8");
            return new Tuple<Vector2?, bool>(null, true);
        }
        else
        {
            // UpdateTowerPreview(tilePosition, "ff2031b8");
            return new Tuple<Vector2?, bool>(tilePosition, true);
        }
    }
}