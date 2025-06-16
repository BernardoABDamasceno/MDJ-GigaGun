using System.Collections;
using UnityEngine;

public class RPG : Gun
{
    [Header("RPG Settings")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileLifetime = 10.0f;

    public override void shoot()
    {
        if (fireRateCooldown) return;
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }
        
        GameObject missile = Instantiate(missilePrefab, transform.position, transform.rotation);
        missile.GetComponent<Missile>().parentTransform = transform;
        StartCoroutine(destroyMissile(missile, missileLifetime));

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

    private IEnumerator destroyMissile(GameObject missile, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (missile != null)
        {
            Destroy(missile);
        }
    }
    public override GunType getGunType()
    {
        return GunType.RPG;
    }
}
