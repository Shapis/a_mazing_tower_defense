using Godot;
using System;

public partial class TowerPreview : Control
{
    [Export]
    private Texture2D? _rangeOverlayTexture;
    private BaseTower? _dragTower;
    private AC.TowerType _towerType;
    private Sprite2D? _rangeTexture;
    private bool _isBuildModeActive = false;
    private Map? _map;

    public sealed override void _Process(float delta)
    {
        if (_map is null)
        {
            return;
        }

        var buildLocationInfo = _map.VerifyBuildLocation(_towerType);
        var currentPosition = buildLocationInfo.Item1;
        var isBlocked = buildLocationInfo.Item2;
        var towerInPosition = buildLocationInfo.Item3;

        if (_isBuildModeActive)
        {
            if (currentPosition is not null && !isBlocked)
            {
                UpdateTower(_map.MapToWorld((Vector2i)currentPosition), "1eff0096");
            }
            else if (currentPosition is null)
            {
                UpdateTower(GetGlobalMousePosition(), "ff2031b8");
            }
            else if (
                towerInPosition?.TowerType == _towerType
                && towerInPosition.UpgradesToType is not null
            )
            {
                UpdateTower(_map.MapToWorld((Vector2i)currentPosition), "216cffb8");
            }
            else
            {
                UpdateTower(_map.MapToWorld((Vector2i)currentPosition), "ff2031b8");
            }
        }
    }

    private void SetTower(AC.TowerType towerType)
    {
        _towerType = towerType;
        _dragTower = GetNode<AC>("/root/AC").GetTower(towerType);

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
        Position = GetGlobalMousePosition();
        AddChild(_dragTower, true);
    }

    private void RemoveDragTower()
    {
        _dragTower!.Free();
        _rangeTexture!.Free();
    }

    private void UpdateTower(Vector2 tilePosition, string colorHex)
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

    internal void InitiateBuildModePreview(AC.TowerType towerName, Map? map)
    {
        SetTower(towerName);
        _isBuildModeActive = true;
        _map = map;
    }

    internal void EndBuildModePreview()
    {
        _map = null;
        _isBuildModeActive = false;
        RemoveDragTower();
    }
}
