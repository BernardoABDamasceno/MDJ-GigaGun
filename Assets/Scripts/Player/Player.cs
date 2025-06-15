using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float HP = 100.0f;
    [SerializeField] float jumpdrag = 1f;
    [SerializeField] float gundragHorizontal = 2f;
    [SerializeField] float gundragVertical = 10f;
    [SerializeField] float gravityAcceleration = 1.25f;
    [SerializeField] float terminalVelocity = 50.0f;
    [SerializeField] float jumpStrength = 18.0f;
    [SerializeField] float airTimer = 0.025f;
    [SerializeField] float jumpColdownTime = 0.475f;
    [SerializeField] float slopeExtraSpeed = 7.0f;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Canvas playerUI;

    [SerializeField] private AudioClip runningSFX;
    private AudioSource audioSource;

    // Audio Mixer Group 
    [Header("Audio Output")]
    [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;


    private Rigidbody rb;
    private Vector3 pushback = Vector3.zero;
    private Vector3 jumpVector = Vector3.zero;
    private Vector3 movementDir = Vector3.zero;
    private Vector3 gravity = Vector3.zero;
    private bool checkJump = false;
    private bool isGrounded = false;
    private bool isOnSlope = false;
    private bool airtime = true;
    private bool jumpCooldown = false;
    private Vector3 storedVelocity = Vector3.zero;
    private int currentXP = 0;

    float horizontalInput;
    float verticalInput;


    // find angle between player and ground
    private RaycastHit hit = new RaycastHit();

    // Start is called before the first frame illustration
    void Start()
    {
        // Application.targetFrameRate = 120;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // If no AudioSource is found, add one automatically
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Player movement sound shouldn't play immediately
        audioSource.loop = true; // Running sound should loop
        audioSource.clip = runningSFX; // Assign the running SFX

        // Assign the AudioMixerGroup
        if (sfxAudioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxAudioMixerGroup;
        }


        // Ensures game is unpaused and cursor is set correctly at the start of the game scene
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the game window
        Cursor.visible = false; // Hide the cursor
    }

    void Update()
    {
        // if assembly mode, cant move player
        if (CameraManager.isAssemblyMode) return;

        // Get input for horizontal and vertical movement
        // This needs to be raw or the player will keep moving after the key is released because of input smoothing
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right Arrow keys
        verticalInput = Input.GetAxisRaw("Vertical");    // W/S or Up/Down Arrow keys

        if (Input.GetKey(KeyCode.Space)) checkJump = true;

        // Logging player actions for data collection
        if (!PauseManager.isGamePaused || !CameraManager.isAssemblyMode)
        {
            if (!isGrounded && !isOnSlope) JsonFileWriter.sampleData.airTime += Time.deltaTime;
        }
    }

    //  TODO: Refactor
    void FixedUpdate()
    {
        // this is just to stop unnecessary physics calculations
        if (CameraManager.isAssemblyMode) return;

        // Prevent movement if the game is paused
        if (Time.timeScale == 0f)
        {
            rb.velocity = Vector3.zero;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        if (checkJump)
        {
            if ((isGrounded && !jumpCooldown) || (isOnSlope && !jumpCooldown))
            {
                //rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                jumpVector = Vector3.up * jumpStrength;
                airtime = true;
                jumpCooldown = true;
                gravity = Vector3.zero;
                Invoke("jumpCooldownOver", jumpColdownTime);
            }
            checkJump = false;
        }

        // get mov dir
        movementDir = transform.forward * verticalInput + transform.right * horizontalInput;

        movementDir.y = 0.0f; // flat ground

        movementDir = movementDir.normalized * moveSpeed;

        if (isGrounded && !isOnSlope)
        {
            // print("Grounded");
            rb.velocity = movementDir + pushback + jumpVector / 1.5f;
        }
        else if (isOnSlope) // effectively not grounded, but on a slope
        // i want to fucking mcshoot myself, this shit is so fucking stupid and it doesnt even work wtf
        {
            if (!Physics.SphereCast(transform.position, 0.95f, Vector3.down, out hit, 0.25f))
            {
                isOnSlope = false;
            }
            Vector3 groundNormal = hit.normal;
            Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(Vector3.up, groundNormal), groundNormal).normalized;
            float dotDirectionSlope = Vector3.Dot(movementDir.normalized, slopeDirection);

            //downhill
            if (dotDirectionSlope > 0.1f)
            {
                // print("On Slope Downhill");
                if (movementDir.magnitude <= 0.1) // if player isnt clicking anything
                {
                    rb.velocity = pushback + jumpVector / 1.5f;
                }
                else
                {
                    //this aint quite right yet but it works
                    movementDir.y = -gravity.y;
                    movementDir = movementDir.normalized * (moveSpeed + slopeExtraSpeed);
                    rb.velocity = movementDir + pushback + jumpVector / 1.5f;
                }
            }
            //uphill
            else if (dotDirectionSlope < -0.1f)
            {
                // print("On Slope Uphill");
                rb.velocity = movementDir + pushback + jumpVector / 1.5f;
            }
            else if (dotDirectionSlope == 0.0f)
            {
                // print("On Slope falling");
                isOnSlope = false;
                rb.velocity = movementDir + pushback + jumpVector / 1.5f;
            }
            // in case of fucky wucky
            else
            {
                // print("fucky wucky ):");
                rb.velocity = movementDir - gravity + pushback + jumpVector / 1.5f;
            }
        }
        else if (rb.velocity.y > 0.5f || rb.velocity.y < -0.5f || !airtime) // this check might be a bit goofy
        {
            // print("In Air");
            rb.velocity = new Vector3(rb.velocity.x * 0.93f + movementDir.x * 0.07f,
                                     0, rb.velocity.z * 0.93f + movementDir.z * 0.07f)
                                     + pushback + jumpVector - gravity;

            // this whole section needs refactoring, however, no time
            // i believe the correct way to do this is to straight up not use the OnCollision methods and just check ourselves if its in ground or not
            // so for now this will do as a bandaid fix
            if (Physics.CheckSphere(transform.position + Vector3.down * 0.6f, 0.45f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
            {
                isOnSlope = true;
            }
        }
        else
        {
            // print("jump air time");
            rb.velocity = new Vector3(rb.velocity.x * 0.93f + movementDir.x * 0.07f,
                                     0, rb.velocity.z * 0.93f + movementDir.z * 0.07f) + pushback;
            Invoke("airTimeOver", airTimer);
        }

        float pushY = pushback.y;
        pushback = Vector3.MoveTowards(new Vector3(pushback.x, 0, pushback.z), Vector3.zero, gundragHorizontal);
        pushback += Vector3.MoveTowards(new Vector3(0, pushY, 0), Vector3.zero, gundragVertical);
        jumpVector = Vector3.MoveTowards(jumpVector, Vector3.zero, jumpdrag);

        if (pushback.Equals(Vector3.zero) && jumpVector.Equals(Vector3.zero) && !isGrounded && !isOnSlope)
        {
            gravity = Vector3.MoveTowards(gravity, new Vector3(0, terminalVelocity, 0), gravityAcceleration);
        }

        bool isMovingHorizontally = (horizontalInput != 0 || verticalInput != 0);
        bool isGroundedOrOnSlope = isGrounded || isOnSlope;

        if (isMovingHorizontally && isGroundedOrOnSlope)
        {
            // If moving and grounded, and sound isn't already playing, play it
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // If not moving, or in air, or not grounded, stop the sound
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    // TODO: Refactor
    // for some reason, this does not actually restore the velocity
    // i had to go to bed so i will fix this later
    public void paused()
    {
        // Time.timeScale = 0.0f; this one is not a good solution

        storedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
    }
    public void unpaused()
    {
        // Time.timeScale = 1.0f;

        rb.velocity = storedVelocity;
    }
    // TODO: Refactor

    public void applyPushback(Vector3 pushback)
    {
        if (pushback.x < 0.2f && pushback.x > -0.2f)
        {
            pushback.x = 0.0f;
        }
        if (pushback.y < 0.2f && pushback.y > -0.2f)
        {
            pushback.y = 0.0f;
        }
        if (pushback.z < 0.2f && pushback.z > -0.2f)
        {
            pushback.z = 0.0f;
        }
        // rb.AddForce(pushback, ForceMode.Impulse);
        this.pushback += pushback;
        gravity = Vector3.zero;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) // 6 is the ground layer
        {
            if (Physics.SphereCast(transform.position, 0.95f, Vector3.down, out hit, 0.25f))
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
                gravity = Vector3.zero;
            }

            isGrounded = true;
            jumpVector = Vector3.zero;

            // Logging player actions for data collection
            FindObjectOfType<JsonFileWriter>().WriteToJson();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6) // 6 is the ground layer
        {

            if (Physics.SphereCast(transform.position, 0.95f, Vector3.down, out hit, 0.25f) && !jumpCooldown)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
                gravity = Vector3.zero;
            }

            isGrounded = false;
        }
    }
    void airTimeOver()
    {
        airtime = false;
        gravity = Vector3.zero;
        jumpVector = Vector3.zero;
    }
    void jumpCooldownOver()
    {
        jumpCooldown = false;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.6f, 0.45f);
    }

    void gainXP(int xp)
    {
        currentXP += xp;
        // sick of the warning
        if (currentXP <= -1)
        {
            currentXP -= 50;
            cameraManager.SendMessage("levelUp");
        }
    }

    void getHit(float damage)
    {
        //print("Player got hit for " + damage + " damage");
        HP -= damage;
        playerUI.GetComponentInChildren<TextMeshProUGUI>().text = HP + "/200 HP";
        cameraManager.SendMessage("flashRed");
        if (HP <= 0)
        {
            // Player death logic
            Die();
        }
    }

    // Handles the player's death sequence
    private void Die()
    {
        //Debug.Log("Player has died!");

       // Load the desired scene
        SceneManager.LoadScene("GameOver");

    }
}