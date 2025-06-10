using System.Collections;
using UnityEngine;

public class Flamethrower : Gun
{
    [Header("Flamethrower Settings")]
    [SerializeField] private GameObject flames;
    bool isfiring = false;

    void Start()
    {
        flames.SetActive(false);
    }

    public override void shoot() {}

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("Flamethrower is firing");
            isfiring = true;
            flames.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            print("Flamethrower stopped firing");
            isfiring = false;
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

            Vector3 forwardNormalized = -transform.forward.normalized;
            Vector3 kickbackOutput = new Vector3(
                                    forwardNormalized.x * kickbackXZ,
                                    forwardNormalized.y * kickbackY,
                                    forwardNormalized.z * kickbackXZ
                                    );

            //player.SendMessage("applyPushback", kickbackOutput);
        }
    }

    public override GunType getGunType()
    {
        return GunType.Flamethrower;
    }
}