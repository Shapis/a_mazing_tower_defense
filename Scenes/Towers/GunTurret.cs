using Godot;
using System;

public partial class GunTurret : BaseTower
{
    public override AC.TowerType TowerType => AC.TowerType.GunTurret;

    public override AC.TowerType? UpgradesToType => AC.TowerType.DualGunTurret;

    protected override void Shoot(BaseEnemy target)
    {
        target.OnHit(Damage);
        AnimationPlayer?.Play("Fire");
    }
}
