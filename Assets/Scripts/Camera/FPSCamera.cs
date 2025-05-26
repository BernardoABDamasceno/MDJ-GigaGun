using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] Transform player;
    [SerializeField] Transform holder;
    private Vector2 rotation; // Rotation of the camera

    float xRotation;
    float yRotation;

    // Update is called once per frame
    void Update()
    {
        yRotation += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; // Update rotation based on mouse input
        xRotation -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; // Update rotation based on mouse input

        xRotation = Mathf.Clamp(xRotation, -90, 90); // Clamp the x rotation to prevent flipping

        holder.transform.localEulerAngles = new Vector3(xRotation, yRotation, 0); // Apply rotation to the camera
        player.localEulerAngles = new Vector3(0, yRotation, 0); // Apply rotation to the orientation
    }

    public void SetSensitivity(float sensitivity) { this.sensitivity = sensitivity; }
}
