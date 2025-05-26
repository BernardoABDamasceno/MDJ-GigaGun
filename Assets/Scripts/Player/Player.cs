using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 300.0f;
    [SerializeField] float jumpStregth = 300.0f;
    private Rigidbody rb;
    private MeshRenderer rdr;
    private Vector3 movementDir;
    private bool checkJump;
    private Vector3 storedVelocity;

    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rdr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // if assembly mode, cant move player
        if (CameraManager.isAssemblyMode) return;

        // Get input for horizontal and vertical movement
        horizontalInput = Input.GetAxis("Horizontal"); // Typically A/D or Left/Right Arrow keys
        verticalInput = Input.GetAxis("Vertical");   // Typically W/S or Up/Down Arrow keys

        if (Input.GetKeyDown(KeyCode.Space)) checkJump = true;
    }

    void FixedUpdate()
    {
        // this is just to stop unnecessary physics calculations
        if (CameraManager.isAssemblyMode) return;

        float fixedDeltaTime = Time.fixedDeltaTime;
        movementDir = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        movementDir = movementDir * moveSpeed * fixedDeltaTime;

        rb.velocity = new Vector3(movementDir.x, rb.velocity.y, movementDir.z);

        if (checkJump)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 2 / 2 + 0.1f, 1 << 6))
            {
                rb.AddForce(Vector3.up * jumpStregth, ForceMode.Impulse);
            }
            checkJump = false;
        }

    }

    // for some reason, this does not actually restore the velocity
    // i had to go to bed so i will fix this later
    public void paused()
    {
        // Time.timeScale = 0.0f; this one is not a good solution

        storedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }
    public void unpaused()
    {
        // Time.timeScale = 1.0f;

        rb.useGravity = true;
        rb.velocity = storedVelocity;
    }
    public void applyPushback(Vector3 pushback)
    {
        rb.AddForce(pushback, ForceMode.Impulse);
    }
}
