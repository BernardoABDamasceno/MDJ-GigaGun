using UnityEngine;

public class PlasmaBall : MonoBehaviour
{
    private Transform parentTransform;
    private Rigidbody rb;
    [SerializeField] private float plasmaOrbSpeed = 0.5f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float extraDistance = 1.0f;
    private Vector3 playerForward;
    void Start()
    {
        parentTransform = GameObject.FindGameObjectWithTag("Cameraholder").transform;
        playerForward = parentTransform.forward;
        transform.position += playerForward * extraDistance;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position + (playerForward * plasmaOrbSpeed);
        rb.MovePosition(position);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "PlayerGroup" && other.tag != "PlasmaOrb" && other.tag != "Cameraholder" && other.tag != "Gun" && other.tag != "ConnectionPoint" && other.tag != "Missile")
        {
            if (other.tag == "Enemy")
            {
                other.SendMessage("takeDamage", damage);
            }
            Destroy(gameObject);
        }
    }

}
