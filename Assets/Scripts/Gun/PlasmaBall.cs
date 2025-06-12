using UnityEngine;

public class PlasmaBall : MonoBehaviour
{
    private Transform parentTransform;
    private Rigidbody rb;
    [SerializeField] private float plasmaOrbSpeed = 0.5f;
    [SerializeField] private float damage = 5.0f;
    [SerializeField] private float extraDistance = 1.0f;
    [SerializeField] private float plasmaOrbLifetime = 5.0f;

    private Vector3 playerForward;
    void Start()
    {
        parentTransform = GameObject.FindGameObjectWithTag("Cameraholder").transform;
        playerForward = parentTransform.forward;
        transform.position += playerForward * extraDistance;
        rb = GetComponent<Rigidbody>();

        Invoke("ExistentialTimeOut", plasmaOrbLifetime);
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position + (playerForward * plasmaOrbSpeed);
        rb.MovePosition(position);

    }

    void OnTriggerEnter(Collider other)
    {
        //things that SHOULDNT make the projectile destroy itself
        if (other.CompareTag("PlayerGroup") ||
            other.CompareTag("PlasmaOrb") ||
            other.CompareTag("Cameraholder") ||
            other.CompareTag("Gun") ||
            other.CompareTag("ConnectionPoint") ||
            other.CompareTag("Missile") ||
            other.CompareTag("Flamethrower"))
        { return; }

        if (other.CompareTag("Enemy"))
        {
            other.SendMessage("takeDamage", damage);
        }

        Destroy(gameObject);
    }

    private void ExistentialTimeOut()
    {
        Destroy(gameObject);
    }

}
