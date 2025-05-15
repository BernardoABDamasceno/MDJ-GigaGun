using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This script allows the player to shoot a gun using left mouse button

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint; // The point where the bullet spawns
    public GameObject bulletPrefab;
    public float bulletSpeed = 100f;
    public Transform playerCamera;

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Default Fire key is Left Mouse Button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the fire point's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Get the bullet's Rigidbody (if it has one) and apply force
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = playerCamera.forward * bulletSpeed;
        }

        // Optionally destroy the bullet after some time
        Destroy(bullet, 30f);
    }
}