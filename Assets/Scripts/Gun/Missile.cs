using UnityEngine;

public class Missile : MonoBehaviour
{
    private Transform parentTransform;
    private Rigidbody rb;
    protected GameObject player;

    [SerializeField] private float missileSpeed = 0.2f;
    [SerializeField] private float hitDamage = 5.0f;
    [SerializeField] private float explosionDamage = 30.0f;
    [SerializeField] private float playerDamage = 5.0f;
    [SerializeField] private float extraDistance = 1.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private float explosionPushbackXZ = 10.0f; // Horizontal pushback
    [SerializeField] private float explosionPushbackY = 30.0f; // Vertical pushback
    private Vector3 playerForward;
    void Start()
    {
        parentTransform = GameObject.FindGameObjectWithTag("Cameraholder").transform;
        player = GameObject.FindGameObjectWithTag("Player");
        playerForward = parentTransform.forward;
        transform.position += playerForward * extraDistance;
        this.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position + (playerForward * missileSpeed);
        rb.MovePosition(position);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "PlayerGroup" && other.tag != "PlasmaOrb" && other.tag != "Missile")
        {
            print(other.tag);
            if (other.tag == "Enemy")
            {
                other.SendMessage("takeDamage", hitDamage);
            }
            Collider[] targets = Physics.OverlapSphere(this.transform.position, explosionRadius);
            foreach (Collider target in targets)
            {
                print("Target: " + target.name + " Tag: " + target.tag);
                Vector3 direction = (target.transform.position - this.transform.position).normalized;
                if (target.tag == "Enemy")
                {
                    target.SendMessage("takeDamage", explosionDamage);
                    //target.SendMessage("enemyPushback", direction * explosionPushback);  Enemies need gravity + velocity to be pushed back  
                }
                if (target.name == "PlayerModel")
                {
                    Vector3 explosionOutput = new Vector3(
                                            direction.x * explosionPushbackXZ,
                                            direction.y * explosionPushbackY, // Reduced vertical pushback for player
                                            direction.z * explosionPushbackXZ
                                            );
                    print("Player hit by explosion");
                    player.SendMessage("getHit", playerDamage);
                    player.SendMessage("applyPushback", explosionOutput);
                }
            }
            Destroy(gameObject);
        }
    }

}
