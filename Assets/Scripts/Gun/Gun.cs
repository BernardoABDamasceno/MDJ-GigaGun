using UnityEditor.Animations;
using UnityEngine;

// THIS SHOULD PROBABLY BE ABSTRACT LATER
public class Gun : MonoBehaviour
{
    private static int idCounter = 0;
    //this is unique
    private int id = 0;

    [SerializeField] private GameObject gunModel;
    //TODO: add all relevant stats

    // public Gun(GameObject gunModel, Transform position)
    // {
    //     this.gunModel = gunModel;
    //     GameObject.Instantiate(gunModel, position.position, position.rotation);
    // }

    void Awake()
    {
        id = idCounter;
        idCounter++;
    }

    public int getId()
    {
        return id;
    }
}