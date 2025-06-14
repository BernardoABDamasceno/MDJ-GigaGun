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
    }

    public override void shoot()
    {
        if (!isfiring)
        {
            isfiring = true;
            flames.SetActive(true);
            flamesPs.Play();
            //if after a second this function isn't called again, is not firing anymore
            Invoke("firingTimeOut", firingTimeOutTime);
        }
        else
        {
            //if it is still firing just reset timer
            CancelInvoke();
            Invoke("firingTimeOut", firingTimeOutTime);
        }
    }
    private void FixedUpdate() {
        if (isfiring)
        {
            if (fireSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireSFX);
            }
                //applied continously   -   not sure if we should even have this
            Vector3 forwardNormalized = -transform.forward.normalized;
            Vector3 kickbackOutput = new Vector3(
                                    forwardNormalized.x * kickbackXZ,
                                    forwardNormalized.y * kickbackY,
                                    forwardNormalized.z * kickbackXZ
                                    );
            player.SendMessage("applyPushback", kickbackOutput);
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
    }
}