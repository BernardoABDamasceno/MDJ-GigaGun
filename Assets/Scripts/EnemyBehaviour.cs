using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 200.0f;
    private Transform player;
    private Vector3 movementdir;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 playerpos = player.position;
        playerpos.y = 0;
        Vector3 enemypos = transform.position;
        enemypos.y = 0;

        print(player.gameObject.name);

        if (Vector3.Distance(enemypos, playerpos) > 3.05f)
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            movementdir = direction * speed * fixedDeltaTime;
            rb.velocity = new Vector3(movementdir.x, rb.velocity.y, movementdir.z);
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}