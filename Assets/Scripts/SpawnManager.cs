using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [Header("Spawner coordinates")]
    public Vector3 position1;
    public Vector3 position2;

    [Header("Time between spawns")]
    [SerializeField] float spawnRate;

    [Header("Number of enemies")]
    public int enemyCount = 1;

    [Header("Is the spawner working ?")]
    [SerializeField] bool isSpawning;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(position1, position2);
        Gizmos.DrawSphere(position1, 0.3f);
        Gizmos.DrawSphere(position2, 0.3f);
    }

    public void CreateSpawner()
    {
        GameObject spawner = (GameObject)Instantiate(spawnerPrefab, gameObject.transform.position, Quaternion.identity,gameObject.transform);
        spawner.name = "EnemySpawner";
        spawner.GetComponent<EnemySpawner>().position1 = position1;
        spawner.GetComponent<EnemySpawner>().position2 = position2;
        spawner.GetComponent<EnemySpawner>().enemyPrefab = enemyPrefab;
        spawner.GetComponent<EnemySpawner>().spawnRate = spawnRate;
        spawner.GetComponent<EnemySpawner>().isSpawning = isSpawning;
        spawner.GetComponent<EnemySpawner>().enemyCount = enemyCount;
    }
}
