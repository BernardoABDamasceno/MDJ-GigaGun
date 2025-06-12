using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Callbacks;

public class Flamecone : MonoBehaviour
{
    protected GameObject player;

    [Header("Flamecone Settings")]
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private float delay = 0.2f;
    private bool tick = true;
    private List<Collider> collidersInTrigger = new List<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!collidersInTrigger.Contains(other))
            {
                collidersInTrigger.Add(other);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        //if its exiting it implies that it entered so a list check for is unnecessery right?
        if (other.CompareTag("Enemy"))
        {
            collidersInTrigger.Remove(other);
        }
    }

    private void Update()
    {
        if (tick)
        {
            //if enemy died onTriggerExit does not trigger, so manual removal is required
            for (int i = 0; i < collidersInTrigger.Count;)
            {
                if (collidersInTrigger[i] == null) collidersInTrigger.RemoveAt(i);
                else i++;
            }
            //after sanitation is safe to do damage
            foreach (Collider enemy in collidersInTrigger)
            {
                enemy.SendMessage("takeDamage", damage);
            }
            tick = false;
            Invoke("tickCooldown", delay);
        }
    }

    private void tickCooldown()
    {
        tick = true;
    }
}
