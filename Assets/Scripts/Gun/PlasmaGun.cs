using System.Collections;
using UnityEngine;

public class PlasmaGun : Gun
{
    [Header("Plasma Gun Settings")]
    [SerializeField] private GameObject plasmaOrbPrefab;
    public override void shoot()
    {
        if (fireRateCooldown) return;

        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        //needs refactoring
        Instantiate(plasmaOrbPrefab, transform.position, transform.rotation);

        Vector3 forwardNormalized = -transform.forward.normalized;
        Vector3 kickbackOutput = new Vector3(
            forwardNormalized.x * kickbackXZ,
            forwardNormalized.y * kickbackY,
            forwardNormalized.z * kickbackXZ
        );
        player.SendMessage("applyPushback", kickbackOutput);

        ps.Play();

        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);
    }

    public override GunType getGunType()
    {
        return GunType.PlasmaGun;
    }
}
