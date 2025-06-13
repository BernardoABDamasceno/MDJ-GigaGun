using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public override void shoot()
    {
        if (fireRateCooldown) return;

        // Plays the assigned SFX
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        int randomNumber = UnityEngine.Random.Range(1, 5);


        //mby randomize this the rays later
        List<Ray> rays = new()
        {
            new Ray(transform.position, transform.forward + transform.right * 0.025f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward - transform.right * 0.025f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward + transform.up * 0.025f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward - transform.up * 0.025f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward + transform.right * 0.0125f *UnityEngine.Random.Range(2, 5) + transform.up * 0.0125f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward - transform.right * 0.0125f *UnityEngine.Random.Range(2, 5) + transform.up * 0.0125f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward + transform.right * 0.0125f *UnityEngine.Random.Range(2, 5) - transform.up * 0.0125f *UnityEngine.Random.Range(2, 5)),
            new Ray(transform.position, transform.forward - transform.right * 0.0125f *UnityEngine.Random.Range(2, 5) - transform.up * 0.0125f *UnityEngine.Random.Range(2, 5))
        };

        // Debug lines for visualizing shotgun spread
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized + transform.right * 0.1f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized - transform.right * 0.1f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized + transform.up * 0.1f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized - transform.up * 0.1f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized + transform.right * 0.05f + transform.up * 0.05f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized - transform.right * 0.05f + transform.up * 0.05f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized + transform.right * 0.05f - transform.up * 0.05f) * 10f, Color.red, 1f);
        Debug.DrawLine(transform.position, transform.position + (transform.forward.normalized - transform.right * 0.05f - transform.up * 0.05f) * 10f, Color.red, 1f);

        foreach (Ray ray in rays)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyBehaviour enemy = hit.collider.GetComponent<EnemyBehaviour>();
                    if (enemy != null)
                    {
                        enemy.takeDamage(damage);
                    }
                }
            }
        }

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
        return GunType.Shotgun;
    }
}