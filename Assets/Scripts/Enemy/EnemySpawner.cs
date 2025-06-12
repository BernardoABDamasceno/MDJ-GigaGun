using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject hulkPrefab;
    [SerializeField] float startDelay = 0.0f;
    [SerializeField] float rateOfSpawns = 10.0f;

    [SerializeField] static int maxEnemies = 100;
    public static int currentEnemies = 0;


    void Start()
    {
        InvokeRepeating("spawnEnemy",startDelay, rateOfSpawns);
    }

    private void spawnEnemy()
    {
        if (CameraManager.isAssemblyMode || currentEnemies >= maxEnemies) return;
        currentEnemies++;
        if (Random.Range(0, 100) <= 15)
        {
            Instantiate(hulkPrefab, transform.position, Quaternion.identity, transform);
        }
        else
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
