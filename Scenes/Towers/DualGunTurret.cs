using Godot;
using System;

public partial class DualGunTurret : BaseTower
{
    public override AC.TowerType TowerType => AC.TowerType.DualGunTurret;

    public override AC.TowerType? UpgradesToType => null;

    int _muzzleTurn = 1;

    protected override void Shoot(BaseEnemy target)
    {
        if (_muzzleTurn > 2)
        {
            _muzzleTurn = 1;
        }
        target.OnHit(Damage);
        AnimationPlayer?.Play($"Fire{_muzzleTurn}");
        _muzzleTurn++;
    }
}
