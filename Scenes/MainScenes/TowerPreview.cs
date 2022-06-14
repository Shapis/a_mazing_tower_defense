using Godot;
using System;

public partial class TowerPreview : Control
{
    [Export] private Texture2D? _rangeOverlayTexture;
    private BaseTower? _dragTower;
    private Sprite2D? _rangeTexture;

    internal void SetTowerPreview(AC.TowerName towerName, Vector2 globalMousePosition)
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

    internal void RemoveDragTower()
    {
        RemoveChild(_dragTower);
        RemoveChild(_rangeTexture);
    }

    internal void UpdateTowerPreview(Vector2 tilePosition, string colorHex)
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
}
