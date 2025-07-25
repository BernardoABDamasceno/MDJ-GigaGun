using UnityEngine;
using static PauseManager; // To check if the game is paused

public class Grenade : MonoBehaviour
{
    [Header("Grenade Stats")]
    [SerializeField] private float explosionDamage = 30.0f;
    [SerializeField] private float playerDamage = 5.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private float explosionPushbackXZ = 10.0f; // Horizontal pushback
    [SerializeField] private float explosionPushbackY = 30.0f; // Vertical pushback

    [Header("Particle FX's")]
    [SerializeField] private ParticleSystem explosionPs;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSFX;
    private AudioSource audioSource;

    private float explosionTimer = 3.0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {

            audioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    void Update()
    {
        if (CameraManager.isAssemblyMode || isGamePaused) return;

        explosionTimer -= Time.deltaTime;

        if (explosionTimer <= 0f)
        {
            // Play explosion sound BEFORE destroying the object
            if (explosionSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(explosionSFX);

            }

            //explosion logic
            //due to structure the to identify the player we need to check for the "Ignore Raycast" layer, which aint optimal
            Collider[] entitiesInRange = Physics.OverlapSphere(
                transform.position,
                explosionRadius,
                LayerMask.GetMask("Enemies", "Ignore Raycast"),
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
                else if(hit.CompareTag("Enemy"))
                {
                    hit.gameObject.SendMessage("takeDamage", explosionDamage);
                }
            }
            //setup explosion and destruction
            explosionPs.Play();
            gameObject.GetComponent<Renderer>().enabled = false;
            Destroy(gameObject, explosionPs.main.startLifetime.constant);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("Grenade collided with: " + collision.gameObject.name);
        if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Grenade") && !collision.gameObject.CompareTag("Gun") && !collision.gameObject.CompareTag("PlasmaOrb")  && !collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        //print("Grenade triggered with: " + other.gameObject.name);
        if (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Grenade") && !other.gameObject.CompareTag("Gun") && !other.gameObject.CompareTag("PlasmaOrb")  && !other.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grenade"))
        {
            GetComponent<SphereCollider>().isTrigger = false;
        }
    }
}