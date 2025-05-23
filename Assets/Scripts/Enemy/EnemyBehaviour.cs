using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 200.0f;
    private Transform player;
    private Vector3 movementdir;
    private Rigidbody rb;
    private ParticleSystem ps;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        // stops behaviour if in assembly mode
        if (CameraManager.isAssemblyMode)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 playerpos = player.position;
        playerpos.y = 0;
        Vector3 enemypos = transform.position;
        enemypos.y = 0;

        if (Vector3.Distance(enemypos, playerpos) > 1.05f)
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            movementdir = direction * speed * fixedDeltaTime;
            rb.velocity = new Vector3(movementdir.x, rb.velocity.y, movementdir.z);
        }
    }

    public void Death()
    {
        ps.Play();
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, ps.main.startLifetime.constant);
    }
}