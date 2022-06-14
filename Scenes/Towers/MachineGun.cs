using Godot;
using System;

public partial class MachineGun : BaseTower
{
    protected override void Shoot(BaseEnemy target)
    {
        target.OnHit(Damage);
        AnimationPlayer?.Play("Fire");
    }
}