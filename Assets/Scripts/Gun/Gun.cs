using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Identifiers
    private static int idCounter = 0;
    //this is unique
    private int id = 0;

    private GameObject player;
    private GameObject recoilManager;

    // Gun properties
    [Header("Gun Stats")]
    [SerializeField] private float kickback = 5.0f;
    [SerializeField] private float fireRate = 0.5f;
    // Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float snapiness;

    private bool fireRateCooldown = false;

    // Gun Model
    [Header("Gun Model")]
    [SerializeField] private GameObject gunModel;

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

        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawLine(transform.position, transform.position + transform.forward.normalized * 10f, Color.red, 1f);

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

        player.SendMessage("applyPushback", -transform.forward.normalized * kickback);
        recoilManager.SendMessage("fireRecoil", new Vector3(recoilX, recoilY, recoilZ));

        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);
    }
    public int getId() { return id; }
    public Vector3 getRecoil() { return new Vector3(recoilX, recoilY, recoilZ); }
    public float getKickback() { return kickback; }

    private void finishFireRateCooldown() { fireRateCooldown = false; }

}