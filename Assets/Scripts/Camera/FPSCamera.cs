using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 5.0f;
    private Vector2 rotation; // Rotation of the camera

    // Update is called once per frame
    void Update()
    {
        rotation.y += Input.GetAxis("Mouse X") * sensitivity; // Update rotation based on mouse input
        rotation.x -= Input.GetAxis("Mouse Y") * sensitivity; // Update rotation based on mouse input

        rotation.x = Mathf.Clamp(rotation.x, -90, 90); // Clamp the x rotation to prevent flipping

        transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0); // Apply rotation to the camera
    }

    public void SetSensitivity(float sensitivity) { this.sensitivity = sensitivity; }    

}
