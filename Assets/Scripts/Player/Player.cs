using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float jumpdrag = 1f;
    [SerializeField] float gundrag = 2f;
    [SerializeField] float gravityAcceleration = 1.25f;
    [SerializeField] float terminalVelocity = 25.0f;
    [SerializeField] float jumpStrength = 18.0f;
    [SerializeField] float airTimer = 0.025f;
    [SerializeField] float jumpColdownTime = 0.475f;
    private Rigidbody rb;
    private Vector3 pushback = Vector3.zero;
    private Vector3 jumpVector = Vector3.zero;
    private Vector3 movementDir = Vector3.zero;
    private Vector3 gravity = Vector3.zero;
    private bool checkJump = false;
    private bool isGrounded = false;
    private bool airtime = true;
    private bool jumpCooldown = false;
    private Vector3 storedVelocity = Vector3.zero;

    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 120;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // if assembly mode, cant move player
        if (CameraManager.isAssemblyMode) return;

        // Get input for horizontal and vertical movement
        // This needs to be raw or the player will keep moving after the key is released because of input smoothing
        horizontalInput = Input.GetAxisRaw("Horizontal"); // Typically A/D or Left/Right Arrow keys
        verticalInput = Input.GetAxisRaw("Vertical");   // Typically W/S or Up/Down Arrow keys

        if (Input.GetKey(KeyCode.Space)) checkJump = true;
    }

    //  TODO: Refactor
    void FixedUpdate()
    {
        // this is just to stop unnecessary physics calculations
        if (CameraManager.isAssemblyMode) return;

        if (checkJump)
        {
            if (isGrounded && !jumpCooldown)
            {
                //rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                jumpVector = Vector3.up * jumpStrength;
                airtime = true;
                jumpCooldown = true;
                Invoke("jumpCooldownOver", jumpColdownTime);
            }
            checkJump = false;
        }

        movementDir = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        movementDir = movementDir * moveSpeed;

        if (isGrounded)
        {
            rb.velocity = new Vector3(movementDir.x, 0, movementDir.z) + pushback + jumpVector / 1.5f;
        }
        else if (rb.velocity.y > 0.5f || rb.velocity.y < -0.5f || !airtime)
        {
            rb.velocity = new Vector3(rb.velocity.x * 0.93f + movementDir.x * 0.07f,
                                    0, rb.velocity.z * 0.93f + movementDir.z * 0.07f)
                                    + pushback + jumpVector - gravity;
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x * 0.93f + movementDir.x * 0.07f,
                                    0, rb.velocity.z * 0.93f + movementDir.z * 0.07f) + pushback;
            Invoke("airTimeOver", airTimer);
        }
        gravity = Vector3.MoveTowards(gravity, new Vector3(0, terminalVelocity, 0), gravityAcceleration);
        pushback = Vector3.MoveTowards( pushback, Vector3.zero, gundrag);
        jumpVector = Vector3.MoveTowards(jumpVector, Vector3.zero, jumpdrag);

    }

    //  TODO: Refactor
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
    //  TODO: Refactor
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
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) // 6 is the ground layer
        {
            isGrounded = true;
            gravity = Vector3.zero;
            jumpVector = Vector3.zero;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6) // 6 is the ground layer
        {
            isGrounded = false;
            gravity = Vector3.zero;
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
}
