using UnityEngine;
using UnityEngine.InputSystem;

// This script allows the player to shoot a gun using left mouse button

public class PlayerShooting : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Default Fire key is Left Mouse Button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBehaviour enemy = hit.collider.GetComponent<EnemyBehaviour>();
                if (enemy != null)
                {
                    enemy.Death();
                }
            }
        }
    }
}