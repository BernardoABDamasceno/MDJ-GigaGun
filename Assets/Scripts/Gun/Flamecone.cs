using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Flamecone : MonoBehaviour
{
    protected GameObject player;

    [Header("Flamecone Settings")]
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private float delay = 0.2f;
    private bool tick = true;
    private bool doublestop = false;
    private List<Collider> collidersInTrigger = new List<Collider>();
    
    void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        if (other.CompareTag("Enemy"))
        {
            if (!collidersInTrigger.Contains(other))
            {
                collidersInTrigger.Add(other);
            }
            other.SendMessage("takeDamage", damage);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !doublestop && tick && collidersInTrigger.Contains(other))
        {
            StartCoroutine(Delay(delay));
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && collidersInTrigger.Contains(other))
        {
            collidersInTrigger.Remove(other);
        }
    }
    IEnumerator Delay(float delayTime)
    {
        if (doublestop)
            yield break;

        tick = false;
        doublestop = true;

        collidersInTrigger.RemoveAll(col => col == null);

        foreach (Collider col in collidersInTrigger)
        {
            if (col.CompareTag("Enemy"))
            {
                col.SendMessage("takeDamage", damage);
                print("DMG");
            }
        }
        yield return new WaitForSeconds(delayTime);
        tick = true;
        doublestop = false;
        print("Delay ended");
    }
}
