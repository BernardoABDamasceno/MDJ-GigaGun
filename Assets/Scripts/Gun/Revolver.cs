using UnityEngine;

public class Revolver : Gun
{

    public override void shoot()
    {
        base.shoot();
    }

    public override GunType getGunType()
    {
        return GunType.Revolver;
    }
}