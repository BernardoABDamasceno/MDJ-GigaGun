using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    [SerializeField] Transform target; // The target to follow
    [SerializeField] static float distance = 5.0f; // Distance from the target
    [SerializeField] float sensitivity = 5.0f; // Mouse sensitivity
    private Vector3 offset = new Vector3(distance, distance, distance); // Offset from the target
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position + offset;
        transform.LookAt(target); // Make the camera look at the target
    }

    // Update is called once per frame
    void Update()
    {
        float x_input = Input.GetAxis("Mouse X");
        float y_input = Input.GetAxis("Mouse Y");

        transform.RotateAround(target.position, Vector3.up, x_input * sensitivity); // Rotate around the target
        
        if (transform.rotation.eulerAngles.x > -(Mathf.PI/2) && transform.rotation.x < Mathf.PI/2)
            transform.RotateAround(target.position, transform.right, y_input * sensitivity); // Rotate around the target
        else
            transform.rotation = new Quaternion(
                Mathf.Clamp(transform.rotation.x, -(Mathf.PI/2), Mathf.PI/2),
                transform.rotation.y,
                0,
                transform.rotation.w
            ); // Lock the camera rotation on the z-axis

    }
}
