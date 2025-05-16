using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float speed = 2.0f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }
    void Update()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * speed * Time.deltaTime;
    }
}