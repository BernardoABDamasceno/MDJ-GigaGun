using System.Collections;
using UnityEngine;

public class Flamecone : MonoBehaviour
{
    protected GameObject player;

    [Header("Flamecone Settings")]
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private float delay = 0.2f;
    private bool tick = false;
    private bool doublestop = false;
    
    void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        if (other.tag == "Enemy")
        {
            other.SendMessage("takeDamage", damage);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            StartCoroutine(Delay(delay));
            if (tick)
            {
                tick = false;
                other.SendMessage("takeDamage", damage);
                print("DMG");
            }    
        }
    }
    IEnumerator Delay(float delayTime)
    {
        if (doublestop)
            yield break;
        print("Delay started");
        doublestop = true;
        yield return new WaitForSeconds(delayTime);
        tick = true;
        doublestop = false;
        print("Delay ended");
    }
}
