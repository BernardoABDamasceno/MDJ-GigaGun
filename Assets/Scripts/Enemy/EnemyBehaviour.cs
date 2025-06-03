using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player;
    private GameObject playerObj;
    private NavMeshAgent agent;
    private ParticleSystem ps;
    private bool isDead = false;

    [SerializeField] private float health = 15.0f;

    private float ogSpeed;

    public float detectionRadius = 125f; // How far the enemy notices the player
    public float wanderSpeed = 3.5f; // Speed when wandering
    public float chaseSpeed = 5f;    // Speed when chasing player

    // State machine enum
    public enum EnemyState
    {
        Wandering,
        Chasing
    }

    public EnemyState currentState;

    // Variables for wandering
    public float wanderMovementRange = 5f; // Range for local "left and right" movement

    public float minNewWanderTargetDelay = 1.5f; // Minimum delay for picking a new local wander target
    public float maxNewWanderTargetDelay = 3.0f; // Maximum delay for picking a new local wander target
    private float newWanderTargetDelay; // This will hold the random delay for *this* enemy

    private float wanderTimer;
    private Vector3 currentWanderTarget;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
        ps = GetComponentInChildren<ParticleSystem>();
        agent = GetComponent<NavMeshAgent>();
        ogSpeed = agent.speed;

        // Start in Wandering state
        currentState = EnemyState.Wandering;

        // Assign a random delay for *this* enemy
        newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
        // Initialize timer with a random offset (so they don't all start choosing new targets at once)
        wanderTimer = Random.Range(0f, newWanderTargetDelay);

        SetNewWanderTarget();
    }

    void Update()
    {
        // stops behaviour if in assembly mode
        if (CameraManager.isAssemblyMode || isDead)
        {
            agent.speed = 0;
            return;
        }

        // State machine logic
        switch (currentState)
        {
            case EnemyState.Wandering:
                WanderState();
                break;
            case EnemyState.Chasing:
                ChaseState();
                break;
        }
    }

    void WanderState()
    {
        agent.speed = wanderSpeed; // Uses wanderSpeed

        // Check if player is within detection radius
        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            // If player is within detection radius, switch to chasing state
            currentState = EnemyState.Chasing;
            return;
        }

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            SetNewWanderTarget();
            // Re-assign a new random delay for the *next* cycle
            newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
            wanderTimer = newWanderTargetDelay;
        }

        agent.SetDestination(currentWanderTarget); // Uses currentWanderTarget
    }

    void ChaseState()
    {
        agent.speed = chaseSpeed;

        // Check if player is *outside* detection radius
        if (Vector3.Distance(transform.position, player.position) > detectionRadius)
        {
            currentState = EnemyState.Wandering;
            SetNewWanderTarget(); // Set a new wander target when returning to wander
            // Re-assign a new random delay and reset timer for wandering
            newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
            wanderTimer = newWanderTargetDelay;
            return; // Immediately switch to wander
        }

        agent.SetDestination(player.position);
    }

    void SetNewWanderTarget() //
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderMovementRange;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, wanderMovementRange, NavMesh.AllAreas))
        {
            currentWanderTarget = hit.position;
        }
        else
        {
            currentWanderTarget = transform.position;
        }
    }
    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        ps.Play();
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        playerObj.SendMessage("gainXP", 10);
        isDead = true;
        Destroy(gameObject, ps.main.startLifetime.constant);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Enemy collided with player");
            collision.gameObject.SendMessage("getHit", 5.0f);
        }
    }
}