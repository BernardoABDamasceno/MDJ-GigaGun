using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player;
    private GameObject playerObj;
    private NavMeshAgent agent;
    private bool isDead = false;
    private Animator animator;

    [SerializeField] private float health = 15.0f;
    [SerializeField] private ParticleSystem bloodSplaterDeath;
    [SerializeField] private ParticleSystem bloodSplatterHit;

    private float ogSpeed;

    public float detectionRadius = 125f; // How far the enemy notices the player
    public float wanderSpeed = 3.5f; // Speed when wandering
    public float chaseSpeed = 5f;    // Speed when chasing player

    // Attack parameters
    public float attackRange = 2.0f; // Distance to trigger attack
    public float attackCooldown = 1.5f; // Time between attacks
    private float currentAttackCooldownTimer; // Timer for the cooldown

    //offset for the model rotation (to make it face the right way)
    public Vector3 modelRotationOffset = new Vector3(0, 180, 0);

    // State machine enum
    public enum EnemyState
    {
        Wandering,
        Chasing,
        Attacking
    }

    public EnemyState currentState;

    // Variables for wandering
    public float wanderMovementRange = 5f; // How far the enemy can wander from its current position

    // Variables for local wander target picking
    public float minNewWanderTargetDelay = 1.5f; // Minimum delay for picking a new local wander target
    public float maxNewWanderTargetDelay = 3.0f; // Maximum delay for picking a new local wander target
    private float newWanderTargetDelay; // This will hold the random delay for *this* enemy

    private float wanderTimer;
    private Vector3 currentWanderTarget;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ogSpeed = agent.speed;

        // Start in Wandering state
        currentState = EnemyState.Wandering;

        // Assign a random delay for *this* enemy
        newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
        // Initialize timer with a random offset (so they don't all start choosing new targets at once)
        wanderTimer = Random.Range(0f, newWanderTargetDelay);

        SetNewWanderTarget();

        // Initialize attack cooldown timer
        currentAttackCooldownTimer = 0f;

        //TO fix!! (temporary fix for now)
        // Disables Update Rotation on the NavMeshAgent
        if (agent != null)
        {
            agent.updateRotation = false;
        }
    }

    void Update()
    {
        // If in assembly mode or dead, stop movement and animation
        if (CameraManager.isAssemblyMode || isDead)
        {
            agent.speed = 0;
            animator.SetBool("isWalking", false);
            return;
        }

        // State machine logic for movement
        switch (currentState)
        {
            case EnemyState.Wandering:
                WanderState();
                break;
            case EnemyState.Chasing:
                ChaseState();
                break;
            case EnemyState.Attacking:
                AttackState();
                break;
        }

        // Rotates towards movement only when not in Attacking state
        if (currentState != EnemyState.Attacking)
        {
             RotateTowardsMovement();
        }

        // Update the animator based on the agent's velocity.
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    void WanderState()
    {
        agent.speed = wanderSpeed;
        agent.isStopped = false; // Ensure agent is not stopped while wandering

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
            newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
            wanderTimer = newWanderTargetDelay;
        }

        agent.SetDestination(currentWanderTarget);
    }

    void ChaseState()
    {
        agent.speed = chaseSpeed;
        agent.isStopped = false; // Ensure agent is not stopped while chasing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is *outside* detection radius
        if (distanceToPlayer > detectionRadius)
        {
            currentState = EnemyState.Wandering;
            SetNewWanderTarget(); // Set a new wander target when returning to wander
            newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
            wanderTimer = newWanderTargetDelay;
            return;
        }

        // Check if player is within attack range AND attack cooldown is over
        if (distanceToPlayer <= attackRange && currentAttackCooldownTimer <= 0f)
        {
            currentState = EnemyState.Attacking;
            currentAttackCooldownTimer = attackCooldown; // Reset cooldown
            agent.isStopped = true; // Stop movement to play attack animation
            animator.SetBool("isWalking", false);
            animator.SetTrigger("AttackTrigger"); // Trigger the attack animation
            return; // Immediately switch to attack state
        }

        // If not attacking and still chasing, continue moving towards the player
        agent.SetDestination(player.position);
    }

    void AttackState()
    {
        // Keeps the agent stopped during attack
        agent.isStopped = true;
        animator.SetBool("isWalking", false); // Ensures walking animation is off

        // Makes sure the enemy is facing the player while attacking
        RotateTowardsTarget(player.position);

        // Decrement cooldown timer
        currentAttackCooldownTimer -= Time.deltaTime;

        // When cooldown is over, transition back to chasing or wandering
        if (currentAttackCooldownTimer <= 0f)
        {
            agent.isStopped = false; // Allow movement again
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRadius)
            {
                currentState = EnemyState.Chasing; // Player is still in range, resume chasing
            }
            else
            {
                currentState = EnemyState.Wandering; // Player too far, go back to wandering
                SetNewWanderTarget(); // Set a new wander target when returning to wander
                newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
                wanderTimer = newWanderTargetDelay;
            }
        }
    }

    void SetNewWanderTarget()
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

    // handles rotating the enemy towards its movement direction, with an offset
    void RotateTowardsMovement()
    {
        // Only rotate based on velocity if the agent is actively moving and not stopped manually
        if (agent.velocity.magnitude > 0.1f && !agent.isStopped)
        {
            // Calculates the target rotation based on the agent's velocity, ignoring Y-axis
            Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z).normalized;

            // Ensure there's a valid direction to look at (prevents Quaternion.LookRotation(Vector3.zero) errors)
            if (horizontalVelocity.magnitude > 0.01f)
            {
                Quaternion baseRotation = Quaternion.LookRotation(horizontalVelocity);
                Quaternion finalRotation = baseRotation * Quaternion.Euler(modelRotationOffset);
                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, 10f * Time.deltaTime);
            }
        }
    }

    // handles rotating the enemy towards a specific target
    void RotateTowardsTarget(Vector3 targetPosition)
    {
        // Calculate the direction to the target, ignoring Y-axis for horizontal rotation
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0; // Ensure rotation is only on the horizontal plane

        if (directionToTarget.magnitude > 0.01f) // Ensure it's not a zero vector
        {
            Quaternion baseRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion finalRotation = baseRotation * Quaternion.Euler(modelRotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, 10f * Time.deltaTime);
        }
    }

    public void takeDamage(float damage)
    {
        print("HIT");
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
        else
        {
            bloodSplatterHit.Play();
        }
    }

    public void Death()
    {
        bloodSplaterDeath.Play();
        // It's often good to stop the NavMeshAgent completely when dying
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Clear any residual velocity
            agent.enabled = false; // Disable the NavMeshAgent component
        }
        // Stop the enemy's movement and animations
        animator.SetBool("isWalking", false);
        animator.SetTrigger("DieTrigger"); // Trigger the death animation

        gameObject.GetComponent<Renderer>().enabled = false; // Hide the model
        gameObject.GetComponent<Collider>().enabled = false; // Disable collision

        playerObj.SendMessage("gainXP", 10);
        isDead = true;

        Destroy(gameObject, bloodSplaterDeath.main.startLifetime.constant); // Destroy after particle system finishes
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