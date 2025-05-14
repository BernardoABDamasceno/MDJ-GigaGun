using UnityEngine;
using UnityEngine.InputSystem;


public class CameraBehaviour : MonoBehaviour
{
    private Camera cam;

     // The target to follow
    [SerializeField] float distance = 12.0f; // Distance from the target
    [SerializeField] float sensitivity = 5.0f; // Mouse sensitivity
    [SerializeField] float lerpDuration = 1.0f; // Duration of the transition
    [SerializeField] Transform gigaGun;
    
    private Transform target = null;
    private Vector2 rotation; // Rotation of the camera
    private bool assemblyMode = true;
    private bool switchingTarget = false;
    private float elapsedTime = 0f;
    private Vector3 initialLerpPosition;
    private Vector2 storedMousePos;

    void Awake()
    {
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        target = gigaGun;

        camfpsMode();
    }

    // Update is called once per frame
    void Update()
    {
        // Check inputs
        if(Input.GetKeyDown(KeyCode.Y)) camAssemblyMode(); // Switch to assembly mode
        if(Input.GetKeyDown(KeyCode.U)) camfpsMode(); // Switch to FPS mode
        if(Input.GetKeyDown(KeyCode.I) && target != gigaGun) switchTarget(gigaGun);


        if (!switchingTarget)
        {
            if (assemblyMode)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.CompareTag("ConnectionPoint"))
                        {
                            switchTarget(hit.transform);
                        }
                    }
                }
                else if (Input.GetMouseButton(0)) 
                {
                    camMovement(true);
                    if(Cursor.visible) 
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
            }
            else camMovement(false);
        }
        else
        {
            if (elapsedTime < lerpDuration)
            {
                
                // Smoothly transition to the new target
                Vector3 lerpValue = Vector3.Lerp(initialLerpPosition, target.position - transform.forward * distance, easeOutQuart(elapsedTime / lerpDuration));
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

    private void camMovement(bool isOrbital)
    {
        rotation.y += Input.GetAxis("Mouse X") * sensitivity; // Update rotation based on mouse input
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity; // Update rotation based on mouse input

        rotation.x = Mathf.Clamp(rotation.x, -90, 90); // Clamp the x rotation to prevent flipping

        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0); // Apply rotation to the camera   

        if(isOrbital) transform.position = target.position - transform.forward * distance;
    }

    public void switchTarget(Transform newTarget){ 
        target = newTarget; 
        switchingTarget = true; 
        initialLerpPosition = transform.position; // Store the initial position of the camera
    }

    private void camfpsMode()
    {
        assemblyMode = false;
        Cursor.visible = false;
    }
    
    private void camAssemblyMode() { 
        assemblyMode = true; 
        // Set initial transformations for orbital camera
        transform.position = target.position - transform.forward * distance; // Set initial position of the camera
        rotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y); // Initialize rotation
    }

    private float easeOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);   
    }
}
