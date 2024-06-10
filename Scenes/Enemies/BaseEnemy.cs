using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

public abstract partial class BaseEnemy : PathFollow2D
{
    // [Signal]
    public event Action<object> OnTargetReachedEvent;

    // [Signal]
    public event Action<object> OnEnemyDiedEvent;

    [Export]
    public float Speed { get; private set; } = 100f;

    [Export]
    public float Health { get; set; } = 100f;

    [Export]
    private NodePath? _characterBody2DPath;
    private CharacterBody2D? _characterBody2D;
    private TextureProgressBar? _healthBar;
    private Vector2 _healthBarOffset = new Vector2(-30, 28);

    [Export]
    public EnemyRarity Rarity { get; private set; } = EnemyRarity.Common;

    public enum EnemyRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public sealed override void _Ready()
    {
        _characterBody2D = GetNode<CharacterBody2D>(_characterBody2DPath);
        _healthBar =
            GetChildren().FirstOrDefault(x => x is TextureProgressBar) as TextureProgressBar;
        if (_healthBar is null)
        {
            GD.PrintErr(this, "Health bar not found");
            return;
        }
        _healthBar.MaxValue = Health;
        _healthBar.Value = Health;
        _healthBar.TopLevel = true;
        __Ready();
    }

    protected virtual void __Ready() { }

    public sealed override void _PhysicsProcess(double delta)
    {
        Move(delta);
    }

    private void Move(double delta)
    {
        
        Progress += (float)(Speed * delta);
        _healthBar!.Position = GlobalPosition + _healthBarOffset;

        if (ProgressRatio >= 1)
        {
            OnTargetReachedEvent?.Invoke(this);
            DieImmediately();
        }
    }

    public void OnHit(float Damage)
    {
        Health -= Damage;
        if (_healthBar is null)
        {
            GD.PrintErr(this, "Health bar not found");
            return;
        }
        _healthBar.Value = Health;
        __OnHit();
        if (Health <= 0)
        {
            Die();
        }
    }

    private void DieImmediately()
    {
        QueueFree();
    }

    private async void Die()
    {
        if (_characterBody2D is null)
        {
            GD.PrintErr(this, "Character body not found");
            return;
        }
        OnEnemyDiedEvent?.Invoke(this);
        _characterBody2D.QueueFree();
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        QueueFree();
    }

    protected virtual void __OnHit() { }
}
