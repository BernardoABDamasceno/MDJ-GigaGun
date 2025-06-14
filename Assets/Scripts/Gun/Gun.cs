using UnityEngine;
using UnityEngine.Audio; // Required for AudioMixerGroup

public abstract class Gun : MonoBehaviour
{
    // Identifiers
    private static int idCounter = 0;
    //this is unique
    private int id = 0;
    public enum GunType
    {
        Revolver,
        PlasmaGun,
        Shotgun,
        Flamethrower,
        RPG,
    }

    [Header("References")]
    protected GameObject player;
    protected ParticleSystem ps;
    protected AudioSource audioSource; // NEW: Reference to the AudioSource

    // Gun properties
    [Header("Gun Stats")]
    [SerializeField] protected float kickbackXZ = 1.5f;
    [SerializeField] protected float kickbackY = 5.0f;
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected float damage = 5.0f;

    // Audio Clip
    [Header("Audio")]
    [SerializeField] protected AudioClip fireSFX;
    // Add this field for the Audio Mixer Group
    [SerializeField] protected AudioMixerGroup sfxAudioMixerGroup; // This will apply to all guns

    protected bool fireRateCooldown = false;


    void Awake()
    {
        id = idCounter;
        idCounter++;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ps = GetComponentInChildren<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();
        // Corrected logic: only add AudioSource if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Assign the AudioMixerGroup here, applies to all derived guns
        if (sfxAudioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxAudioMixerGroup;
        }
    }

    public virtual void shoot()
    {
        if (fireRateCooldown) return;

        // Play the SFX
        if (fireSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }

        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawLine(transform.position, transform.position + transform.forward.normalized * 10f, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBehaviour enemy = hit.collider.GetComponent<EnemyBehaviour>();
                if (enemy != null)
                {
                    enemy.takeDamage(damage);
                }
            }
        }
        Vector3 forwardNormalized = -transform.forward.normalized;
        Vector3 kickbackOutput = new Vector3(
                                            forwardNormalized.x * kickbackXZ,
                                            forwardNormalized.y * kickbackY,
                                            forwardNormalized.z * kickbackXZ
                                            );

        if (player != null) // Keep this null check for 'player' as discussed
        {
            player.SendMessage("applyPushback", kickbackOutput);
        }
        else
        {
            Debug.LogError("Player GameObject is null in Gun. Make sure your Player has the 'Player' tag and is active in the scene.");
        }

        ps.Play();

        fireRateCooldown = true;
        Invoke("finishFireRateCooldown", fireRate);
    }
    public int getId() { return id; }

    protected void finishFireRateCooldown() { fireRateCooldown = false; }

    public abstract GunType getGunType();

}