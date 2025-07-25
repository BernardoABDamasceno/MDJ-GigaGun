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
        Collider[] collisions = Physics.OverlapSphere(transform.position, 0.065f, LayerMask.GetMask("Gun", "Ignore Raycast", "ConnectionPoint"), QueryTriggerInteraction.Collide);

        foreach (Collider col in collisions)
        {
            if (col.gameObject.transform.parent.gameObject != gameObject)
            {
                gameObject.SetActive(false);
                break;
            }
        }
    }

}
