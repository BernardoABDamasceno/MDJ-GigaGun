using UnityEngine;

public class Revolver : Gun
{
    public override void shoot()
    {
        // Plays the assigned SFX
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        base.shoot();
    }

    public override GunType getGunType()
    {
        return GunType.Revolver;
    }
}