using Godot;
using System;

public partial class TowerPreview : Control
{
    [Export]
    private Texture2D? _rangeOverlayTexture;
    private BaseTower? _dragTower;
    private Sprite2D? _rangeTexture;
    private bool _isBuildModeActive = false;
    private Map? _map;
    private AC.TowerType _originalDragTower;
    private AC? _assortedCatalog;
    private float previewModulationAlpha = 170f;

    public sealed override void _Ready()
    {
        _assortedCatalog = GetNode<AC>("/root/AC");
    }

    public sealed override void _Process(float delta)
    {
        if (_map is null)
        {
            return;
        }

        var buildLocationInfo = _map.VerifyBuildLocation(_dragTower!.TowerType);
        var currentPosition = buildLocationInfo.Item1;
        var isBlocked = buildLocationInfo.Item2;
        var towerInPosition = buildLocationInfo.Item3;

        if (_isBuildModeActive)
        {
            if (currentPosition is null)
            {
                if (_dragTower!.TowerType != _originalDragTower)
                {
                    RemoveDragTower();
                    SetTower(_originalDragTower);
                }
                UpdateTower(
                    GetGlobalMousePosition(),
                    _assortedCatalog!.GetColor(AC.ColorPalette.Red, previewModulationAlpha)
                );
            }
            else if (!isBlocked)
            {
                if (_dragTower!.TowerType != _originalDragTower)
                {
                    RemoveDragTower();
                    SetTower(_originalDragTower);
                }
                UpdateTower(
                    _map.MapToWorld((Vector2i)currentPosition),
                    _assortedCatalog!.GetColor(AC.ColorPalette.Green, previewModulationAlpha)
                );
            }
            else if (towerInPosition?.TowerType != _originalDragTower)
            {
                if (_dragTower!.TowerType != _originalDragTower)
                {
                    RemoveDragTower();
                    SetTower(_originalDragTower);
                }
                UpdateTower(
                    _map.MapToWorld((Vector2i)currentPosition),
                    _assortedCatalog!.GetColor(AC.ColorPalette.Red, previewModulationAlpha)
                );
            }
            else if (towerInPosition.UpgradesToType == null)
            {
                if (_dragTower!.TowerType != _originalDragTower)
                {
                    RemoveDragTower();
                    SetTower(_originalDragTower);
                }
                UpdateTower(
                    _map.MapToWorld((Vector2i)currentPosition),
                    _assortedCatalog!.GetColor(AC.ColorPalette.Red, previewModulationAlpha)
                );
            }
            else
            {
                if (_dragTower!.TowerType != towerInPosition.UpgradesToType)
                {
                    RemoveDragTower();
                    SetTower((AC.TowerType)towerInPosition.UpgradesToType);
                }

                UpdateTower(
                    _map.MapToWorld((Vector2i)currentPosition),
                    _assortedCatalog!.GetColor(AC.ColorPalette.Blue, previewModulationAlpha)
                );
            }
        }
    }

    private void SetTower(AC.TowerType towerType)
    {
        _dragTower = GetNode<AC>("/root/AC").GetTower(towerType);

        if (_dragTower is null)
        {
            GD.PrintErr(this, "Failed to load tower");
            return;
        }

        _dragTower.Modulate = _assortedCatalog!.GetColor(
            AC.ColorPalette.Green,
            previewModulationAlpha
        );
        _dragTower.Rotate(-Mathf.Pi / 2);

        _rangeTexture = new Sprite2D();
        float scaling = _dragTower.Range / 600f;
        _rangeTexture.Scale = new Vector2(scaling, scaling);
        _rangeTexture.Texture = _rangeOverlayTexture;
        _rangeTexture.Modulate = _assortedCatalog!.GetColor(
            AC.ColorPalette.Green,
            previewModulationAlpha
        );

        AddChild(_rangeTexture);
        Position = GetGlobalMousePosition();
        AddChild(_dragTower, true);
    }

    private void RemoveDragTower()
    {
        _dragTower!.QueueFree();
        _rangeTexture!.QueueFree();
    }

    private void UpdateTower(Vector2 tilePosition, Color color)
    {
        Position = tilePosition;

        if (_dragTower is null)
        {
            GD.PrintErr(this, "Drag tower is null");
            return;
        }

        if (_dragTower.Modulate != color)
        {
            _dragTower.Modulate = color;
            if (_rangeTexture is null)
            {
                GD.PrintErr(this, "Range texture is null");
                return;
            }
            _rangeTexture.Modulate = color;
        }
    }

    internal void InitiateBuildModePreview(AC.TowerType towerType, Map? map)
    {
        _originalDragTower = towerType;
        SetTower(towerType);
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
