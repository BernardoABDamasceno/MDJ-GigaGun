using UnityEngine;

public class Flamethrower : Gun
{
    [Header("Flamethrower Settings")]
    [SerializeField] private GameObject flames;
    [SerializeField] private ParticleSystem flamesPs;
    bool isfiring = false;

    void Start()
    {
        flames.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void shoot()
    {
        Vector3 forwardNormalized = -transform.forward.normalized;
            Vector3 kickbackOutput = new Vector3(
                                    forwardNormalized.x * kickbackXZ,
                                    forwardNormalized.y * kickbackY,
                                    forwardNormalized.z * kickbackXZ
                                    );
        player.SendMessage("applyPushback", kickbackOutput);
    }

    // this needs to be removed
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("Flamethrower is firing");
            isfiring = true;
            flamesPs.Play();
            flames.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            print("Flamethrower stopped firing");
            isfiring = false;
            flamesPs.Stop();
            flames.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (isfiring)
        {
            if (fireSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireSFX);
            }

            // Always play the fire particle system when isfiring is true
        }
    }

    public override GunType getGunType()
    {
        return GunType.Flamethrower;
    }
}