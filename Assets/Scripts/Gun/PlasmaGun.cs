using System.Collections;
using UnityEngine;

public class PlasmaGun : Gun
{
    [Header("Plasma Gun Settings")]
    [SerializeField] private GameObject plasmaOrbPrefab;
    [SerializeField] private float plasmaOrbLifetime = 5.0f;

    public override void shoot()
    {
        if (fireRateCooldown) return;
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }
        //needs refactoring
        GameObject orb = Instantiate(plasmaOrbPrefab, transform.position, transform.rotation);
        StartCoroutine(destroyPlasmaOrb(orb, plasmaOrbLifetime));

        Vector3 kickbackOutput = new Vector3(
                                 transform.forward.x * kickbackXZ,
                                 transform.forward.y * kickbackY,
                                 transform.forward.z * kickbackXZ
                                 );

        player.SendMessage("applyPushback", kickbackOutput);

        ps.Play();

        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);

    }

    private IEnumerator destroyPlasmaOrb(GameObject orb, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        // Destroy the plasma orb after the specified lifetime
        if (orb != null)
        {
            Destroy(orb);
        }
    }
    public override GunType getGunType()
    {
        return GunType.Revolver;
    }
}
