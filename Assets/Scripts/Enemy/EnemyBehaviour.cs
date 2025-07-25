using UnityEngine;
using UnityEngine.AI;
using static PauseManager; // To check if the game is paused
using UnityEngine.Audio;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player;
    private GameObject playerObj;
    private NavMeshAgent agent;
    private bool isDead = false;
    private Animator animator;

    // audio variables for SFX
    [SerializeField] private AudioClip approachingSFX;
    [SerializeField] private AudioClip attackingSFX;
    [SerializeField] private AudioClip dyingSFX;
    private AudioSource loopingAudioSource;    // For approaching
    private AudioSource oneShotAudioSource;    // For attacks and dying,

    // ---  Audio Control Variables ---
    [Header("Approaching Sound Settings")]
    [Tooltip("Distance at which the approaching sound starts playing.")]
    [SerializeField] private float approachingSoundActivationDistance = 10.0f; // Adjust in Inspector
    [Tooltip("Distance at which the approaching sound stops playing. Should be >= Activation Distance.")]
    [SerializeField] private float approachingSoundDeactivationDistance = 12.0f; // Adjust in Inspector
    [Tooltip("Minimum time (seconds) before the approaching sound can start playing again after stopping.")]
    [SerializeField] private float approachingSoundStartCooldown = 0.5f; // Adjust in Inspector
    private float _approachingSoundCurrentCooldown = 0f; // Internal timer

    // --- Audio Mixer Group ---
    [Header("Audio Output")] // New Header for organization
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;

    [SerializeField] bool isHulk = false;
    [SerializeField] private float health = 15.0f;
    [SerializeField] private ParticleSystem bloodSplaterDeath;
    [SerializeField] private ParticleSystem bloodSplatterHit;
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] float throwAngle = 5.0f;
    [SerializeField] float throwForce = 10.0f;

    private float ogSpeed;

    public float detectionRadius = 125f; // How far the enemy notices the player
    public float wanderSpeed = 3.5f; // Speed when wandering
    public float chaseSpeed = 5f;     // Speed when chasing player

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

        // --- AudioSource Setup ---
        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length == 0)
        {
            loopingAudioSource = gameObject.AddComponent<AudioSource>();
            oneShotAudioSource = gameObject.AddComponent<AudioSource>();
        }
        else if (audioSources.Length == 1)
        {
            // If only one exists, assign it as the looping source and add another for one-shots
            loopingAudioSource = audioSources[0];
            oneShotAudioSource = gameObject.AddComponent<AudioSource>();
        }
        else // if (audioSources.Length >= 2)
        {
            // If two or more exist, use the first two found
            loopingAudioSource = audioSources[0];
            oneShotAudioSource = audioSources[1];
        }

        // --- Assigning AudioMixerGroup to both AudioSources ---
        if (sfxAudioMixerGroup != null)
        {
            if (loopingAudioSource != null)
            {
                loopingAudioSource.outputAudioMixerGroup = sfxAudioMixerGroup;
            }
            if (oneShotAudioSource != null)
            {
                oneShotAudioSource.outputAudioMixerGroup = sfxAudioMixerGroup;
            }
        }

        // Configures the looping AudioSource
        if (loopingAudioSource != null)
        {
            loopingAudioSource.clip = approachingSFX;
            loopingAudioSource.loop = true; // This one will always loop its assigned clip
            loopingAudioSource.playOnAwake = false;
            loopingAudioSource.spatialBlend = 1f; // Full 3D sound
            loopingAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            // ---  AudioSource min/maxDistance based on sound trigger distances ---
            loopingAudioSource.minDistance = 1.0f; // Sound is at full volume up to this distance
            loopingAudioSource.maxDistance = approachingSoundDeactivationDistance; // Sound is inaudible after this distance
            loopingAudioSource.pitch = 1f; // Ensures normal playback speed
        }
        // Configures the one-shot AudioSource
        if (oneShotAudioSource != null)
        {
            oneShotAudioSource.loop = false; // This one should never loop
            oneShotAudioSource.playOnAwake = false; // Don't play immediately
            oneShotAudioSource.spatialBlend = 1f; // Full 3D sound
            oneShotAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            oneShotAudioSource.minDistance = 1.5f;
            oneShotAudioSource.maxDistance = 2.0f;
            oneShotAudioSource.pitch = 1f; // Ensures normal playback speed
        }
        // --- End AudioSource Setup ---

        currentState = EnemyState.Wandering;
        // Assign a random delay for *this* enemy
        newWanderTargetDelay = Random.Range(minNewWanderTargetDelay, maxNewWanderTargetDelay);
        // Initialize timer with a random offset (so they don't all start choosing new targets at once)
        wanderTimer = Random.Range(0f, newWanderTargetDelay);

        // --- Initialize approaching sound cooldown ---
        _approachingSoundCurrentCooldown = 0f;

        SetNewWanderTarget();

        // Initialize attack cooldown timer
        currentAttackCooldownTimer = 0f;

        //TO fix!! (temporary fix for now)
        // Disables Update Rotation on the NavMeshAgent
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        InvokeRepeating("throwGrenade", 0, 10);
    }

    void Update()
    {
        if (isDead)
        {
            if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                agent.enabled = false;
            }

            if (animator.enabled && animator.speed == 0)
            {
                animator.speed = 1;
            }
            // Stop approaching sound immediately on death
            if (loopingAudioSource != null && loopingAudioSource.isPlaying)
            {
                loopingAudioSource.Stop();
                // --- Reset cooldown when sound stops on death ---
                _approachingSoundCurrentCooldown = approachingSoundStartCooldown;
            }
            return;
        }

        // ASSEMBLY MODE OR GAME PAUSED
        if (CameraManager.isAssemblyMode || isGamePaused)
        {
            if (agent != null && agent.enabled)
            {
                agent.speed = 0;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            if (animator.enabled)
            {
                animator.SetBool("isWalking", false);
                animator.ResetTrigger("AttackTrigger");
                animator.speed = 0; // Freeze the animator's playback speed
            }
            // Stop approaching sound when in assembly mode or paused
            if (loopingAudioSource != null && loopingAudioSource.isPlaying)
            {
                loopingAudioSource.Stop();
                // --- Reset cooldown when sound stops on pause ---
                _approachingSoundCurrentCooldown = approachingSoundStartCooldown;
            }
            return; // Exit Update early if in assembly mode or paused
        }

        // If we just exited assembly mode/pause and the animator was frozen, unfreeze it
        if (!CameraManager.isAssemblyMode && !isGamePaused && !isDead && animator.speed == 0)
        {
            animator.speed = 1; // Resume normal animation playback
            if (agent != null && !agent.enabled) // Re-enable agent if it was disabled by assembly/pause mode
            {
                agent.enabled = true;
                agent.isStopped = false; // Allow movement again
                agent.speed = ogSpeed; // Restore original speed
            }
        }

        // --- Approaching Sound Cooldown Update ---
        if (_approachingSoundCurrentCooldown > 0)
        {
            _approachingSoundCurrentCooldown -= Time.deltaTime;
        }
        // --- End Approaching Sound Cooldown Update ---


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

        // Update the animator based on the agent's velocity and handle approaching SFX
        if (animator.speed != 0) // Only update walking if animator is playing normally
        {
            bool isCurrentlyWalking = agent.velocity.magnitude > 0.1f;
            animator.SetBool("isWalking", isCurrentlyWalking);

            // ---  Approaching SFX Play/Stop Logic ---
            if (loopingAudioSource != null && approachingSFX != null && player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // Conditions to START playing the sound: is walking AND within activation distance AND not already playing AND cooldown is over
                if (isCurrentlyWalking && distanceToPlayer < approachingSoundActivationDistance)
                {
                    if (!loopingAudioSource.isPlaying && _approachingSoundCurrentCooldown <= 0f)
                    {
                        loopingAudioSource.Play();
                    }
                }
                // Conditions to STOP playing the sound: currently playing AND (not walking OR outside deactivation distance)
                else
                {
                    if (loopingAudioSource.isPlaying && (distanceToPlayer >= approachingSoundDeactivationDistance || !isCurrentlyWalking))
                    {
                        loopingAudioSource.Stop();
                        _approachingSoundCurrentCooldown = approachingSoundStartCooldown; // Reset cooldown when sound stops
                    }
                }
            }
        }
    }

    void WanderState()
    {
        agent.speed = wanderSpeed;
        agent.isStopped = false; // Ensures agent is not stopped while wandering

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

            // Play attacking SFX on the one-shot AudioSource
            if (oneShotAudioSource != null && attackingSFX != null)
            {
                oneShotAudioSource.PlayOneShot(attackingSFX);
            }
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

        // Stop approaching sound when in attack state, using the dedicated looping source
        if (loopingAudioSource != null && loopingAudioSource.isPlaying)
        {
            loopingAudioSource.Stop();
            // --- Reset cooldown when sound stops on attack ---
            _approachingSoundCurrentCooldown = approachingSoundStartCooldown;
        }

        // Makes sure the enemy is facing the player while attacking
        RotateTowardsTarget(player.position);

        // cooldown timer
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

            // Ensure there's a valid direction to look at before applying rotation
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
        if (isDead || CameraManager.isAssemblyMode || isGamePaused) return;

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

    private void throwGrenade()
    {
        if (isDead || CameraManager.isAssemblyMode || isGamePaused) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // BIGGEST PILE OF SHIT I'VE PROBABLY DONE SO FAR BUT FUCK IT
        if (distanceToPlayer <= 50.0f && currentAttackCooldownTimer <= 0f)
        {
            if (Random.Range(0, 100) <= 33)
            {
                if (isHulk)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject grenade = Instantiate(grenadePrefab, transform.position + Vector3.up * 2.0f, Quaternion.identity);
                        grenade.GetComponent<Rigidbody>().AddForce(
                            (player.position - transform.position + Vector3.up * throwAngle + transform.right * 2.0f * (2 - i)) * throwForce,
                            ForceMode.Impulse
                        );
                    }
                }
                else
                {
                    GameObject grenade = Instantiate(grenadePrefab, transform.position + Vector3.up * 2.0f, Quaternion.identity);
                    grenade.GetComponent<Rigidbody>().AddForce((player.position - transform.position + Vector3.up * throwAngle) * throwForce, ForceMode.Impulse);
                }
                currentState = EnemyState.Attacking;
                currentAttackCooldownTimer = attackCooldown; // Reset cooldown
                agent.isStopped = true; // Stop movement to play attack animation
                animator.SetBool("isWalking", false);
                animator.SetTrigger("AttackTrigger"); // Trigger the attack animation

                // Play attacking SFX on the one-shot AudioSource
                if (oneShotAudioSource != null && attackingSFX != null)
                {
                    oneShotAudioSource.PlayOneShot(attackingSFX);
                }
                return; // Immediately switch to attack state
            }
        }
    }

    public void Death()
    {
        //Debug.Log("Enemy Death() method called! Time: " + Time.time); 
        EnemySpawner.currentEnemies--;

        // Stop any looping audio when dying
        if (loopingAudioSource != null && loopingAudioSource.isPlaying)
        {
            loopingAudioSource.Stop();
            //Debug.Log("Looping audio stopped on death.");
        }

        // Play dying SFX on the one-shot AudioSource
        if (oneShotAudioSource != null && dyingSFX != null)
        {
            //Debug.Log("Attempting to play dying SFX: " + dyingSFX.name);
            oneShotAudioSource.PlayOneShot(dyingSFX);
            //Debug.Log("Dying SFX PlayOneShot called.");
        }
        else
        {
            //Debug.LogError("ERROR: Dying SFX cannot be played. oneShotAudioSource null: " + (oneShotAudioSource == null) + ", dyingSFX null: " + (dyingSFX == null));
        }

        bloodSplaterDeath.Play(); // The particle system still plays

        // Immediately stop agent movement and disable its component
        // This makes the enemy 'dead' from a gameplay perspective
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        isDead = true; // Mark as dead to stop Update logic

        // Trigger the death animation
        if (animator != null && animator.enabled)
        {
            animator.speed = 1; // Ensure animator is running normally
            animator.SetBool("isWalking", false);
            animator.ResetTrigger("AttackTrigger");
            animator.SetTrigger("DieTrigger"); // Trigger the death animation
        }

        // Hide the model and disable its collider immediately
        // The enemy is effectively gone visually and interactively
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        playerObj.SendMessage("gainXP", 10); // Give XP

        // calculated destruction delay based on sound length
        float destructionDelay = dyingSFX != null ? dyingSFX.length : 1f; // Default to 1 second if no sound is set

        Destroy(gameObject, destructionDelay);
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