using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private ParticleSystem ps;
    private bool isDead = false;

    private float ogSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ps = GetComponentInChildren<ParticleSystem>();
        agent = GetComponent<NavMeshAgent>();
        ogSpeed = agent.speed;
    }

    void Update()
    {
        // stops behaviour if in assembly mode
        if (CameraManager.isAssemblyMode || isDead)
        {
            agent.speed = 0;
            return;
        } else agent.speed = ogSpeed;


        agent.SetDestination(player.position);
    }

    public void Death()
    {
        ps.Play();
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        isDead = true;
        Destroy(gameObject, ps.main.startLifetime.constant);
    }
}