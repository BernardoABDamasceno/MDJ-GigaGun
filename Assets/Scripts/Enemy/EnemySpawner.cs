using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float startDelay = 0.0f;
    [SerializeField] float rateOfSpawns = 10.0f;


    void Start()
    {
        InvokeRepeating("spawnEnemy",startDelay, rateOfSpawns);
    }

    private void spawnEnemy()
    {
        if (CameraManager.isAssemblyMode) return;
        Instantiate(enemyPrefab, transform.position, Quaternion.identity, this.transform);
    }
}
