using UnityEditor;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    private static int idCounter = 0;
    //this is unique
    private int id;
    private bool interactable = false;

    // public ConnectionPoint(Gun parent, Transform position)
    // {
    //     this.parent = parent;
    //     this.position = position;
    //     id = idCounter;
    //     idCounter++;
    // }

    void Awake()
    {
        id = idCounter;
        idCounter++;
    }

    void Start()
    {
        confirmCollisions();
    }

    public int getId()
    {
        return id;
    }
    public bool isInteractable()
    {
        return interactable;
    }
    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
        if (interactable)
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = Color.green;
        }
        else
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }

    public void confirmCollisions()
    {
        if (Physics.SphereCast(transform.position, 0.11f, Vector3.up, out RaycastHit hit, 0f))
        {
            print(hit.collider.tag);
            if (hit.collider.tag == "ConnectionPoint" || hit.collider.tag == "Gun")
            {
                print("DESTROY");
                Destroy(gameObject);
            }
        }
        else
        {
            print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAH");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.11f);
    }

}
