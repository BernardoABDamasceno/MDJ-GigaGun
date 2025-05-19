using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 300.0f;
    [SerializeField] float jumpStregth = 300.0f;
    private Camera cam;
    private Rigidbody rb;
    private MeshRenderer rdr;
    private Vector3 movementDir;
    private bool checkJump;
    private Vector3 storedVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rdr = GetComponent<MeshRenderer>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // if assembly mode, cant move player
        if (CameraManager.isAssemblyMode) return;

        // Get input for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal"); // Typically A/D or Left/Right Arrow keys
        float verticalInput = Input.GetAxis("Vertical");   // Typically W/S or Up/Down Arrow keys

        //there is a bug here, you slow down as you look up or down, since forward tends to zero when looking up or downs
        // possible fix, just have a son empty object that only follows camera on the xz plane perhaps?
        movementDir = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        if (Input.GetKeyDown(KeyCode.Space)) checkJump = true;
    }

    void FixedUpdate()
    {
        // this is just to stop unnecessary physics calculations
        if (CameraManager.isAssemblyMode) return;

        float fixedDeltaTime = Time.fixedDeltaTime;
        movementDir = movementDir * moveSpeed * fixedDeltaTime;

        rb.velocity = new Vector3(movementDir.x, rb.velocity.y, movementDir.z);

        if (checkJump)
        {
            if (Physics.Raycast(transform.position, Vector3.down, rdr.bounds.size.y / 2 + 0.1f, 1 << 6))
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
}
