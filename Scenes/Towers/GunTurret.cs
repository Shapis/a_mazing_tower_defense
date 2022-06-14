using Godot;
using System;

public partial class GunTurret : BaseTower
{
    protected override void Shoot(BaseEnemy target)
    {
        target.OnHit(Damage);
        AnimationPlayer?.Play("Fire");
    }
}
