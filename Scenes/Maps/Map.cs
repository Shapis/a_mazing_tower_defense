using Godot;
using System;
using System.Linq;

public partial class Map : TileMap
{
	[Export]
	private NodePath _towerContainerPath;
	private Node _towerContainer;

	[Export]
	private NodePath _enemyPath2DPath;
	private EnemyPath2D _enemyPath2D;

	public sealed override void _Ready()
	{
		_towerContainer = GetNode<Node>(_towerContainerPath);
		_enemyPath2D = GetNode<EnemyPath2D>(_enemyPath2DPath);
	}

	private AC.TowerType? GetTowerTypeAt(Vector2 position)
	{
		foreach (var item in _towerContainer!.GetChildren().OfType<BaseTower>())
		{
			if (item.Position == position)
			{
				return item.TowerType;
			}
		}
		return null;
	}

	public BaseTower? GetTowerAt(Vector2 position)
	{
		// this is here so any positions within the tile will return the center of the tile, which is the position that the tower is in
		var temp = LocalToMap(position);
		position = MapToLocal(temp);

		foreach (var item in _towerContainer!.GetChildren().OfType<BaseTower>())
		{
			if (item.Position == position)
			{
				return item;
			}
		}
		return null;
	}

	// Verifies if the build location is valid and builds tower, also verifies if there's already a matching tower in the location ,if so upgrades the tower. Returns whether the tower was sucessful built/upgraded or not
	internal bool VerifyAndBuildTower(AC.TowerType towerType)
	{
		var verification = VerifyBuildLocation(towerType);

		var buildTile = verification.Item1;
		var isBlocked = verification.Item2;
		var blockedByTowerOfType = verification.Item3;

		if (blockedByTowerOfType is not null)
		{
			if (
				towerType == blockedByTowerOfType.TowerType
				&& blockedByTowerOfType.UpgradesToType != null
			)
			{
				blockedByTowerOfType.UpgradeTower();
				return true;
			}
		}

		if (buildTile is null || isBlocked)
		{
			return false;
		}

		var nullSafeBuildTile = (Vector2I)buildTile;

		var newTower = GetNode<AC>("/root/AC").GetTower(towerType);
		if (newTower is null)
		{
			GD.PrintErr("Failed to load tower");
			return false;
		}
		newTower.Position = MapToLocal(nullSafeBuildTile);
		newTower.IsBuilt = true;
		newTower.Rotate(-Mathf.Pi / 2);
		_towerContainer!.AddChild(newTower, true);
		var ac = GetNode<AC>("/root/AC");
		SetCell((int)AC.MapLayerName.Towers, nullSafeBuildTile, 1, new Vector2I(0, 0));
		EraseCell((int)AC.MapLayerName.Props, nullSafeBuildTile);
		return true;
	}

	internal void RemoveTowerAt(Vector2 vector2)
	{
		var tower = GetTowerAt(vector2);
		if (tower is null)
		{
			return;
		}
		tower.QueueFree();
		var ac = GetNode<AC>("/root/AC");
		EraseCell((int)AC.MapLayerName.Towers, LocalToMap(vector2));
	}

	// The Vector2i returns null if outside the map, or the tile index if in the map, and the bool returns whether the tile is blocked or not. And if it is blocked by a tower Ac.TowerType returns the Tower that is blocking it.
	internal Tuple<Vector2I?, bool, BaseTower?> VerifyBuildLocation(AC.TowerType towerType)
	{
		Vector2 mousePosition = GetGlobalMousePosition();

		BaseTower? towerInPosition = null;
		var currentTile = LocalToMap(mousePosition);
		var tilePosition = MapToLocal(currentTile);

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
			if (i == (int)AC.MapLayerName.Props)
			{
				continue;
			}
			else if (i == (int)AC.MapLayerName.Towers)
			{
				if (GetCellSourceId(i, currentTile, true) != -1)
				{
					isCellBlocked = true;
					var ac = GetNode<AC>("/root/AC");

					MapToLocal(currentTile);
					towerInPosition = GetTowerAt(MapToLocal(currentTile));
				}
				continue;
			}
			if (GetCellSourceId(i, currentTile, true) != -1)
			{
				isCellBlocked = true;
			}
		}

		SetCell((int)AC.MapLayerName.TowerPreviews, currentTile, 1, new Vector2I(0, 0));

		// If the path couldnt be generated return false and dont build the path2d.
		var isReachable = _enemyPath2D!.GeneratePath();

		ClearLayer((int)AC.MapLayerName.TowerPreviews);

		if (!isCellBlocked && doesCellExist && isReachable)
		{
			// UpdateTowerPreview(tilePosition, "1eff0096");
			return new Tuple<Vector2I?, bool, BaseTower?>(currentTile, false, towerInPosition);
		}
		else if (!doesCellExist)
		{
			// UpdateTowerPreview(mousePosition, "ff2031b8");
			return new Tuple<Vector2I?, bool, BaseTower?>(null, true, towerInPosition);
		}
		else
		{
			// UpdateTowerPreview(tilePosition, "ff2031b8");
			return new Tuple<Vector2I?, bool, BaseTower?>(currentTile, true, towerInPosition);
		}
	}
}
