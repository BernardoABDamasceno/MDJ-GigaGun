using System.Collections;
using UnityEngine;

public class Flamethrower : Gun
{
    [Header("Flamethrower Settings")]
    [SerializeField] private GameObject flames;
    private bool flamesOn = false;

    void Start()
    {
        flames.SetActive(flamesOn);
    }

    public override void shoot()
    {
        if (fireRateCooldown) return;
        // Plays the assigned SFX
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        flamesOn = !flamesOn;
        flames.SetActive(flamesOn);

        Vector3 forwardNormalized = -transform.forward.normalized;
        Vector3 kickbackOutput = new Vector3(
                                 forwardNormalized.x * kickbackXZ,
                                 forwardNormalized.y * kickbackY,
                                 forwardNormalized.z * kickbackXZ
                                 );

        player.SendMessage("applyPushback", kickbackOutput);

        // Ensures ParticleSystem is found and played
        if (ps != null)
        {
            ps.Play();
        }
        else
        {
            Debug.LogWarning("ParticleSystem not found on " + gameObject.name + ". Make sure it's a child object or assigned.");
        }
        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);
    }

    public override GunType getGunType()
    {
        return GunType.Flamethrower;
    }
}