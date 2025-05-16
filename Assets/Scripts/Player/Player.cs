using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rdr = GetComponent<MeshRenderer>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Get input for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal"); // Typically A/D or Left/Right Arrow keys
        float verticalInput = Input.GetAxis("Vertical");   // Typically W/S or Up/Down Arrow keys

        movementDir = (cam.transform.forward * verticalInput + cam.transform.right * horizontalInput).normalized;
        if (Input.GetKeyDown(KeyCode.Space)) checkJump = true;
    }

    void FixedUpdate()
    {
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
}
