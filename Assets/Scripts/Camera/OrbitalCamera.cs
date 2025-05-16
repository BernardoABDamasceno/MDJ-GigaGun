using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitalCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform gigaGun;
    [SerializeField] float nonFocusedDistance = 12.0f; // Distance from the target
    [SerializeField] float focusedDistance = 3.0f;
    [SerializeField] float scrollScale = 1.0f;
    [SerializeField] float lerpDuration = 1.0f; // Duration of the transition
    [SerializeField] float sensitivity = 5.0f;
    
    private Vector2 rotation; // Rotation of the camera
    private Transform target = null;
    private bool switchingTarget = false;
    private float elapsedTime = 0f;
    private Vector3 initialLerpPosition;
    private Vector2 storedMousePos;
    private float maxScrollNFDistance = 15.0f;
    private float minScrollNFDistance = 2.5f;
    private float currentDistance;

    void Start()
    {
        target = gigaGun;
        currentDistance = nonFocusedDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && target != gigaGun)
        {
            resetAssemblyMode();
            gigaGun.gameObject.SendMessage("cancelInsertGun");
        } // Reset to assembly mode        

        if (!switchingTarget)
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
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
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
                rotation.y += Input.GetAxis("Mouse X") * sensitivity; // Update rotation based on mouse input
                rotation.x -= Input.GetAxis("Mouse Y") * sensitivity; // Update rotation based on mouse input

                rotation.x = Mathf.Clamp(rotation.x, -90, 90); // Clamp the x rotation to prevent flipping

                transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0); // Apply rotation to the camera
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

    public void switchTarget(Transform newTarget)
    {
        target = newTarget;
        switchingTarget = true;
        initialLerpPosition = transform.position; // Store the initial position of the camera
        currentDistance = focusedDistance;
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

    public void SetSensitivity(float sensitivity) { this.sensitivity = sensitivity; }    

}
