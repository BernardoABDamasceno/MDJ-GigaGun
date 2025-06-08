using UnityEngine;

public class SMG : MonoBehaviour
{
    // Identifiers
    private static int idCounter = 0;
    //this is unique
    private int id = 0;

    private GameObject player;
    private GameObject recoilManager;
    private ParticleSystem ps;
    private AudioSource audioSource;

    // SMG properties
    [Header("SMG Stats")]
    [SerializeField] private float kickbackXZ = 1.5f;
    [SerializeField] private float kickbackY = 5.0f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float damage = 2.0f;

    // Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float snapiness;

    // Audio Clip
    [Header("Audio")]
    [SerializeField] private AudioClip fireSFX;

    private bool fireRateCooldown = false;

    void Awake()
    {
        id = idCounter;
        idCounter++;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        recoilManager = GameObject.FindGameObjectWithTag("RecoilManager");
        ps = GetComponentInChildren<ParticleSystem>();
        
        // AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void shoot()
    {
        if (fireRateCooldown) return;

        // Play the assigned  SFX
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawLine(transform.position, transform.position + transform.forward.normalized * 10f, Color.red, 1f);

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
        Vector3 forwardNormalized = -transform.forward.normalized;
        Vector3 kickbackOutput = new Vector3(
                                 forwardNormalized.x * kickbackXZ,
                                 forwardNormalized.y * kickbackY,
                                 forwardNormalized.z * kickbackXZ
                                 );

        player.SendMessage("applyPushback", kickbackOutput);
        recoilManager.SendMessage("fireRecoil", new Vector3(recoilX, recoilY, recoilZ));

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
    public int getId() { return id; }

    private void finishFireRateCooldown() { fireRateCooldown = false; }

}