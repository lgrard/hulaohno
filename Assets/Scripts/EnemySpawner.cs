using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemySpawner : MonoBehaviour
{
    [Header("The object to spawn")]
    public GameObject enemyPrefab;

    [Header("Spawner coordinates")]
    public Vector3 position1;
    public Vector3 position2;

    [Header("Time between spawns")]
    public float spawnRate;

    [Header("Number of enemies")]
    public int enemyCount = 1;
    public int enemyRemaining = 1;

    [Header("Is the spawner working ?")]
    public bool isSpawning;

    [HideInInspector]
    public bool noMoreEnemies = false;

    [HideInInspector]
    public GameObject linkedCameraCheckpoint =null;

    private void OnEnable()
    {
        enemyRemaining = enemyCount;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (enemyCount > 0 && isSpawning)
        {
            Vector3 spawnPos = Vector3.Lerp(position1, position2, Random.Range(0f, 1f));

            if (!Physics.CheckSphere(spawnPos, 0.5f))
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                if(enemy.TryGetComponent<Enemy>(out Enemy enemyScriptSolo))
                    enemyScriptSolo.spawner = this;

                if (enemy.TryGetComponent<EnemyDuet>(out EnemyDuet enemyDuet))
                    enemyDuet.spawner = this;

                enemyCount--;

                yield return new WaitForSeconds(spawnRate * Random.Range(0.8f, 1f));
            }

            else
                yield return null;
        }

        isSpawning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position1, position2);
        Gizmos.DrawSphere(position1, 0.15f);
        Gizmos.DrawSphere(position2, 0.15f);
    }
}
