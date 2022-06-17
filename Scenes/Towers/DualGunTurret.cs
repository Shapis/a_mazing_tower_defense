using Godot;
using System;

public partial class DualGunTurret : BaseTower
{
    public override AC.TowerType TowerType => AC.TowerType.DualGunTurret;

    public override AC.TowerType? UpgradesToType => null;

    protected override void Shoot(BaseEnemy target)
    {
        //throw new NotImplementedException();
    }
}
