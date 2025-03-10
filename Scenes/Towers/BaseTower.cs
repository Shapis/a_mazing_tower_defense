using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class BaseTower : Node2D
{
    public abstract AC.TowerType TowerType { get; }
    public abstract AC.TowerType? UpgradesToType { get; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; private set; } = "";

    [Export]
    public float Range { get; private set; } = 600f;

    [Export]
    public float RateOfFire { get; private set; } = 1f;

    [Export]
    public float Damage { get; private set; } = 20f;
    public bool IsBuilt { get; set; } = false;

    [Export]
    private NodePath? _rangeCollisionShape2DPath;
    private CollisionShape2D? _rangeCollisionShape2D;

    [Export]
    private NodePath? _animationPlayerPath;
    protected AnimationPlayer? AnimationPlayer { get; private set; }
    private List<BaseEnemy> _targets = new List<BaseEnemy>();
    private BaseEnemy? _currentTarget;
    private bool _isReloaded = true;

    [Export]
    public TowerRarity Rarity { get; private set; } = TowerRarity.Common;

    public enum TowerRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public void UpgradeTower()
    {
        var ac = GetNode<AC>("/root/AC");
        if (UpgradesToType is not null)
        {
            var newTower = ac.GetTower((AC.TowerType)UpgradesToType!);
            newTower.Position = Position;
            newTower.IsBuilt = true;
            newTower.Rotate(-Mathf.Pi / 2);
            GetParent().AddChild(newTower);
            QueueFree();
        }
        else
        {
            GD.Print("Tower has been upgraded to the max!");
        }
    }

    public sealed override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>(_animationPlayerPath);
        _rangeCollisionShape2D = GetNode<CollisionShape2D>(_rangeCollisionShape2DPath);
        CircleShape2D? circleShape = _rangeCollisionShape2D.Shape as CircleShape2D;
        if (circleShape is null)
        {
            GD.PrintErr(this, "Range overlay is not a circle shape");
            return;
        }
        circleShape.Radius = Range * 0.5f;

        __Ready();
    }

    protected virtual void __Ready() { }

    public sealed override void _PhysicsProcess(double delta)
    {
        if (_targets.Count > 0 && IsBuilt)
        {
            SelectTarget();
            if (!AnimationPlayer!.IsPlaying())
            {
                Turn();
            }
            if (_isReloaded && _currentTarget is not null)
            {
                StartShootRoutine(_currentTarget);
            }
        }
        else
        {
            _currentTarget = null;
        }
    }

    private async void StartShootRoutine(BaseEnemy target)
    {
        _isReloaded = false;
        Shoot(target);
        await ToSignal(GetTree().CreateTimer(1 / RateOfFire), "timeout");
        _isReloaded = true;
    }

    protected abstract void Shoot(BaseEnemy target);

    private void SelectTarget()
    {
        _currentTarget = _targets.OrderBy(x => x.Progress).Last();
    }

    private void Turn()
    {
        if (_currentTarget is null)
        {
            GD.PrintErr(this, "Current target is null");
            return;
        }

        GetNode<Sprite2D>("Turret").LookAt(_currentTarget.Position);
    }

    private void OnRangeBodyEntered(Node2D body)
    {
        _targets.Add(body.GetParent<BaseEnemy>());
    }

    private void OnRangeBodyExited(Node2D body)
    {
        _targets.Remove(body.GetParent<BaseEnemy>());
    }
}
