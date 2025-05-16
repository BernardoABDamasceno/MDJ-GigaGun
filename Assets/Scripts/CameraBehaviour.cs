using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraBehaviour : MonoBehaviour
{
    private Camera cam;

     // The target to follow
    [SerializeField] float nonFocusedDistance = 12.0f; // Distance from the target
    [SerializeField] float focusedDistance = 3.0f;
    [SerializeField] float sensitivity = 5.0f; // Mouse sensitivity
    [SerializeField] float scrollScale = 1.0f;
    [SerializeField] float lerpDuration = 1.0f; // Duration of the transition
    [SerializeField] float gigaGunXOffset = 1.25f;
    [SerializeField] float gigaGunYOffset = 0.5f;
    [SerializeField] Transform gigaGun;
    [SerializeField] Transform player;

    private Transform target = null;
    private Vector2 rotation; // Rotation of the camera
    private bool assemblyMode = true;
    private bool switchingTarget = false;
    private float elapsedTime = 0f;
    private Vector3 initialLerpPosition;
    private Vector2 storedMousePos;
    private float maxScrollNFDistance = 15.0f;
    private float minScrollNFDistance = 2.5f;
    private float currentDistance;
    

    void Awake()
    {
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        target = gigaGun;
        currentDistance = nonFocusedDistance;
        camfpsMode();
    }

    // Update is called once per frame
    void Update()
    {
        // Check inputs
        if (Input.GetKeyDown(KeyCode.Y) && !assemblyMode) camAssemblyMode(); // Switch to assembly mode
        if (Input.GetKeyDown(KeyCode.U) && assemblyMode) camfpsMode(); // Switch to FPS mode
        if (Input.GetKeyDown(KeyCode.I) && target != gigaGun)
        { 
            resetAssemblyMode();
            gigaGun.gameObject.SendMessage("cancelInsertGun");
        } // Reset to assembly mode
        

        if (!switchingTarget)
        {
            if (assemblyMode)
            {
                if (Input.mouseScrollDelta.y != 0.0f)
                {
                    if (currentDistance >= minScrollNFDistance && currentDistance <= maxScrollNFDistance)
                    {
                        currentDistance -= Input.mouseScrollDelta.y * scrollScale;
                        currentDistance = Mathf.Clamp(currentDistance, minScrollNFDistance, maxScrollNFDistance);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.CompareTag("ConnectionPoint"))
                        {
                            ConnectionPoint cp = hit.collider.GetComponentInParent<ConnectionPoint>();

                            if (cp.isInteractable())
                            {
                                switchTarget(hit.transform);
                                gigaGun.gameObject.SendMessage("insertGun", cp);
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    camMovement();
                    if (Cursor.visible)
                    {
                        storedMousePos = Mouse.current.position.ReadValue();
                        Cursor.visible = false;
                    }

                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Mouse.current.WarpCursorPosition(storedMousePos);
                    Cursor.visible = true;
                }

                transform.position = target.position - transform.forward * currentDistance; // move camera to position where it faces the target
            }
            else
            {
                transform.position = player.position;
                camMovement();
                gigaGun.position = transform.position - transform.up * gigaGunYOffset + transform.forward * gigaGunXOffset;
                gigaGun.transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
            }
                
        }
        else
        {
            if (elapsedTime < lerpDuration)
            {
                
                // Smoothly transition to the new target
                Vector3 lerpValue = Vector3.Lerp(initialLerpPosition, target.position - transform.forward * currentDistance, easeOutQuart(elapsedTime / lerpDuration));
                transform.position = lerpValue;
                elapsedTime += Time.deltaTime; // Increment elapsed time
            }
            else
            {
                switchingTarget = false; // Reset the switching flag
                elapsedTime = 0f; // Reset elapsed time
            }
        }

    }

    private void camMovement()
    {
        rotation.y += Input.GetAxis("Mouse X") * sensitivity; // Update rotation based on mouse input
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity; // Update rotation based on mouse input

        rotation.x = Mathf.Clamp(rotation.x, -90, 90); // Clamp the x rotation to prevent flipping

        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0); // Apply rotation to the camera   
    }

    public void switchTarget(Transform newTarget)
    {
        target = newTarget;
        switchingTarget = true;
        initialLerpPosition = transform.position; // Store the initial position of the camera
        currentDistance = focusedDistance;
    }

    private void camfpsMode()
    {
        assemblyMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gigaGun.gameObject.SendMessage("disableConnectionPoints");
    }

    private void camAssemblyMode()
    {
        assemblyMode = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        gigaGun.gameObject.SendMessage("enableConnectionPoints");
        // Set initial transformations for orbital camera
        transform.position = target.position - transform.forward * currentDistance; // Set initial position of the camera
        rotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y); // Initialize rotation
    }

    private float easeOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);   
    }

    private void resetAssemblyMode()
    {
        switchTarget(gigaGun); 
        currentDistance = nonFocusedDistance;
    }
}
