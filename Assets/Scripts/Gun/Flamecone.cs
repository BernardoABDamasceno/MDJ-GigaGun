using UnityEngine;

public class Flamecone : MonoBehaviour
{
    protected GameObject player;

    [Header("Flamecone Settings")]
    [SerializeField] private float damage = 2.0f;
    void OnTriggerEnter(Collider other)
    {   
        print(other.tag);
        if (other.tag == "Enemy")
        {
            other.SendMessage("takeDamage", damage);
        }
    }
}
