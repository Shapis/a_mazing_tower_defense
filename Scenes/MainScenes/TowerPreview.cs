using Godot;
using System;

public partial class TowerPreview : Control, IBuildModeEvents
{
    [Export] private NodePath? _GameScenePath;
    private GameScene? _gameScene;
    [Export] private Texture2D? _rangeOverlayTexture;
    private BaseTower? _dragTower;
    private Sprite2D? _rangeTexture;
    private bool _isBuildModeActive = false;
    [Export] private NodePath? _mapPath;
    private TileMap? _map;
    private Vector2i? _buildTile;

    public sealed override void _Ready()
    {
        _map = GetNode<TileMap>(_mapPath);
        _gameScene = GetNode<GameScene>(_GameScenePath);
        _gameScene.OnBuildModeStartedEvent += OnBuildModeStarted;
        _gameScene.OnBuildModeEndedEvent += OnBuildModeEnded;
    }

    public sealed override void _Process(float delta)
    {
        if (_isBuildModeActive)
        {
            Vector2 mousePosition = GetGlobalMousePosition();

            if (_map is null)
            {
                GD.PrintErr("Map is null");
                return;
            }

            var currentTile = _map.WorldToMap(mousePosition);
            var tilePosition = _map.MapToWorld(currentTile);

            bool doesCellExist = false;
            for (int i = 0; i < 4; i++)
            {
                if (_map.GetCellSourceId(i, currentTile, true) != -1)
                {
                    doesCellExist = true;
                    break;
                }
            }
            bool isCellBlocked = false;
            for (int i = 1; i <= 4; i++)
            {
                if (_map.GetCellSourceId(i, currentTile, true) != -1)
                {
                    isCellBlocked = true;
                }
            }

            if (!isCellBlocked && doesCellExist)
            {
                UpdateTowerPreview(tilePosition, "1eff0096");
                _buildTile = currentTile;
            }
            else if (!doesCellExist)
            {
                UpdateTowerPreview(mousePosition, "ff2031b8");
                _buildTile = null;
            }
            else
            {
                UpdateTowerPreview(tilePosition, "ff2031b8");
                _buildTile = null;
            }
        }
    }

    private void SetTowerPreview(AC.TowerName towerName, Vector2 globalMousePosition)
    {
        _dragTower = GetNode<AC>("/root/AC").GetTower(towerName);

        if (_dragTower is null)
        {
            GD.PrintErr(this, "Failed to load tower");
            return;
        }

        _dragTower.Modulate = new Color("1eff0096");
        _dragTower.Rotate(-Mathf.Pi / 2);

        _rangeTexture = new Sprite2D();
        float scaling = _dragTower.Range / 600f;
        _rangeTexture.Scale = new Vector2(scaling, scaling);
        _rangeTexture.Texture = _rangeOverlayTexture;
        _rangeTexture.Modulate = new Color("1eff0096");


        AddChild(_rangeTexture);
        Position = globalMousePosition;
        AddChild(_dragTower, true);
    }

    private void RemoveDragTower()
    {
        _dragTower!.Free();
        _rangeTexture!.Free();
    }

    private void UpdateTowerPreview(Vector2 tilePosition, string colorHex)
    {
        Position = tilePosition;

        if (_dragTower is null)
        {
            GD.PrintErr(this, "Drag tower is null");
            return;
        }
        if (_dragTower.Modulate != new Color(colorHex))
        {
            _dragTower.Modulate = new Color(colorHex);
            if (_rangeTexture is null)
            {
                GD.PrintErr(this, "Range texture is null");
                return;
            }
            _rangeTexture.Modulate = new Color(colorHex);
        }
    }

    public void OnBuildModeStarted(object sender, AC.TowerName towerName)
    {
        if (_isBuildModeActive)
        {
            OnBuildModeEnded(sender, towerName);
        }
        SetTowerPreview(towerName, GetGlobalMousePosition());
        _isBuildModeActive = true;
    }

    public Vector2i? OnBuildModeEnded(object sender, AC.TowerName towerName)
    {
        RemoveDragTower();
        _isBuildModeActive = false;
        return _buildTile;
    }
}
