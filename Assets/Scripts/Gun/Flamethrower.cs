using UnityEngine;

public class Flamethrower : Gun
{
    [Header("Flamethrower Settings")]
    [SerializeField] private GameObject flames;
    [SerializeField] private ParticleSystem flamesPs;
    [SerializeField] private float firingTimeOutTime = 1.0f;

    private bool isfiring = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        flames.SetActive(false);

        // Setup AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("Flamethrower: AudioSource not found on GameObject.");
            }
        }

        if (fireSFX == null)
        {
            Debug.LogWarning("Flamethrower: fireSFX (AudioClip) is not assigned!");
        }
        else if (audioSource != null)
        {
            audioSource.clip = fireSFX;
            audioSource.loop = true;
        }
    }

    public override void shoot()
    {
        if (!isfiring)
        {
            isfiring = true;
            flames.SetActive(true);
            flamesPs.Play();
             //if after a second this function isn't called again, is not firing anymore

            // Start looping sound
            if (fireSFX != null && audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            Invoke("firingTimeOut", firingTimeOutTime);
        }
        else
        {
            // Still firing, reset timeout
            CancelInvoke();
            Invoke("firingTimeOut", firingTimeOutTime);
        }
    }

    public override GunType getGunType()
    {
        return GunType.Flamethrower;
    }

    private void firingTimeOut()
    {
        isfiring = false;

        flamesPs.Stop();
        flames.SetActive(false);

        // Stop the looping sound
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
