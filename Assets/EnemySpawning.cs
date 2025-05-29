using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    [SerializeField] private float spawnTime;
    private float timer;

    private void SpawnEnemy()
    {
        Instantiate(_enemy, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= spawnTime)
        {
            SpawnEnemy();
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
