using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

     // The target to follow
    [SerializeField] float distance = 10.0f; // Distance from the target
    [SerializeField] float sensitivity = 5.0f; // Mouse sensitivity
    [SerializeField] Transform gigaGun;
    private Transform target = null; // TODO: LOGIC NEEDS TO BE ADDED
    private Vector2 rotation; // Rotation of the camera

    private bool OrbitalCamera = false; 
    

    // Start is called before the first frame update
    void Start()
    {
        //cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transform.position = gigaGun.position + new Vector3(distance, distance, distance); // Set initial position of the camera
        transform.LookAt(gigaGun); // Make the camera look at the target
        rotation = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y); // Initialize rotation
    }

    // Update is called once per frame
    void Update()
    {
        rotation.y += Input.GetAxis("Mouse X") * sensitivity; // Update rotation based on mouse input
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity; // Update rotation based on mouse input

        rotation.x = Mathf.Clamp(rotation.x, -90, 90); // Clamp the x rotation to prevent flipping

        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0); // Apply rotation to the camera

        //positions looking at target
        if(OrbitalCamera) transform.position = gigaGun.position - transform.forward * distance;

        if(Input.GetKeyDown(KeyCode.C)) OrbitalCamera = !OrbitalCamera;
    }

    public void switchTarget(Transform newTarget){ target = newTarget; }

}
