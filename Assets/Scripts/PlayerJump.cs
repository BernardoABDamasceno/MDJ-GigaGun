using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows the player to jump in a 3D space using space key 
[RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 5f;   // Force applied when jumping
    public float fastFallMultiplier = 2.5f; // Multiplier for falling
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the player is grounded using a small sphere cast
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Handle jump input
        if (Input.GetButtonDown("Jump") && isGrounded) // Default Jump key is Space
        {
            // Apply a stronger upward force for a faster jump
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity before jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Apply faster falling when not grounded and falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.velocity += Vector3.down * Physics.gravity.y * (fastFallMultiplier - 1) * Time.deltaTime;
        }
    }
}