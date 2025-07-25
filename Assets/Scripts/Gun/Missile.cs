using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform parentTransform;
    private Rigidbody rb;

    [Header("Rocket Stats")]
    [SerializeField] private float missileSpeed = 0.2f;
    [SerializeField] private float hitDamage = 5.0f;
    [SerializeField] private float explosionDamage = 10.0f;
    [SerializeField] private float playerDamage = 5.0f;
    [SerializeField] private float extraDistance = 1.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private float explosionPushbackXZ = 10.0f; // Horizontal pushback
    [SerializeField] private float explosionPushbackY = 30.0f; // Vertical pushback
    private Vector3 playerForward;
    private bool isExploded = false;

    [Header("Particle FX's")]
    [SerializeField] private ParticleSystem explosionPs;
    [SerializeField] private ParticleSystem trusterPs;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSFX;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }


    void Start()
    {
        playerForward = parentTransform.forward;
        transform.position += playerForward * extraDistance;
        transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isExploded) return;

        Vector3 position = transform.position + (playerForward * missileSpeed);
        rb.MovePosition(position);
    }

    // this might need a refactor
    void OnTriggerEnter(Collider other)
    {
        // if the collision is with anything it should collide with -> return
        // inlcuding player, to prvent midfall shenenigans
        if (other.CompareTag("PlayerGroup") ||
            other.CompareTag("Player") ||
            other.CompareTag("PlasmaOrb") ||
            other.CompareTag("Missile") ||
            other.name == "Flamecone" ||
            other.CompareTag("Gun")) return;

        // direct hit on enemy
        if (other.CompareTag("Enemy")) other.SendMessage("takeDamage", hitDamage);

        //explosion logic
        //due to structure the to identify the player we need to check for the "Ignore Raycast" layer, which aint optimal
        Collider[] entitiesInRange = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            LayerMask.GetMask("Enemy", "Ignore Raycast"),
            QueryTriggerInteraction.Collide
        );
        foreach (Collider hit in entitiesInRange)
        {
            //player specific logic
            if (hit.name == "PlayerModel")
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                Vector3 outputPushVector = new Vector3(
                    direction.x * explosionPushbackXZ,
                    direction.y * explosionPushbackY,
                    direction.z * explosionPushbackXZ
                );
                //doing it this way to avoid doing a FindObjectWithTag search, since our scenes are now quite chunky
                hit.gameObject.GetComponentInParent<Player>().SendMessage("getHit", playerDamage);
                hit.gameObject.GetComponentInParent<Player>().SendMessage("applyPushback", outputPushVector);
            }
            else if (hit.CompareTag("Enemy"))
            {
                hit.gameObject.SendMessage("takeDamage", explosionDamage);
            }
        }

        if (explosionSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSFX);
        }
        
        //stop moving particles
        trusterPs.Stop();
        //setup explosion and destruction
        isExploded = true;                  //stops it from continuing moving while explosion happens
        explosionPs.Play();
        gameObject.GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, explosionPs.main.startLifetime.constant);
    }
    
}
