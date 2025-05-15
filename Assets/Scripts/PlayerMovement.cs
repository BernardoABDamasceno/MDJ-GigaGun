using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows the player to move in a 3D space using WASD or arrow keys.
// It uses Unity's Input system to capture keyboard input and translates the player's position accordingly.

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f; // Adjustable walking speed

    void Update()
    {
        // Get input for horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal"); // Typically A/D or Left/Right Arrow keys
        float verticalInput = Input.GetAxis("Vertical");   // Typically W/S or Up/Down Arrow keys

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Apply movement if there's input
        if (moveDirection != Vector3.zero)
        {
            // Move the player in world space
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
