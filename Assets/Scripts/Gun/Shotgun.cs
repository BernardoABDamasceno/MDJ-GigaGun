using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    // Identifiers
    private static int idCounter = 0;
    //this is unique
    private int id = 0;

    private GameObject player;
    private GameObject recoilManager;

    // Shotgun properties
    [Header("Shotgun Stats")]
    [SerializeField] private float kickbackXZ = 4.5f;
    [SerializeField] private float kickbackY = 10.0f;
    [SerializeField] private float fireRate = 2.0f;
    // Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float snapiness;

    private bool fireRateCooldown = false;

    // Shotgun Model
    [Header("Shotgun Model")]
    [SerializeField] private GameObject shotgunModel;

    void Awake()
    {
        id = idCounter;
        idCounter++;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        recoilManager = GameObject.FindGameObjectWithTag("RecoilManager");
    }

    public void shoot()
    {
        if (fireRateCooldown) return;

        List <Ray> rays = new List<Ray>
        {
            new Ray(transform.position, transform.forward + transform.right * 0.1f),
            new Ray(transform.position, transform.forward - transform.right * 0.1f),
            new Ray(transform.position, transform.forward + transform.up * 0.1f),
            new Ray(transform.position, transform.forward - transform.up * 0.1f),
            new Ray(transform.position, transform.forward + transform.right * 0.05f + transform.up * 0.05f),
            new Ray(transform.position, transform.forward - transform.right * 0.05f + transform.up * 0.05f),
            new Ray(transform.position, transform.forward + transform.right * 0.05f - transform.up * 0.05f),
            new Ray(transform.position, transform.forward - transform.right * 0.05f - transform.up * 0.05f)
        };
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
                        enemy.Death();
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
        recoilManager.SendMessage("fireRecoil", new Vector3(recoilX, recoilY, recoilZ));

        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);
    }
    public int getId() { return id; }

    private void finishFireRateCooldown() { fireRateCooldown = false; }

}