using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDuet : MonoBehaviour
{
    [HideInInspector]
    public EnemySpawner spawner;
    private GameManager gameManager;
    [Header("Objects and values")]
    [SerializeField] GameObject prefabEnemyA;
    [SerializeField] GameObject prefabEnemyB;
    [SerializeField] float respawnDelay = 5f;
    [SerializeField] float spawnSpacing = 2f;
    private bool isSpawning = false;
    private Vector3 nextSpawnPos;

    [HideInInspector]
    [SerializeField] GameObject enemyA;
    [HideInInspector]
    [SerializeField] GameObject enemyB;


    [Header ("Effects")]
    [SerializeField] LineRenderer link;
    [SerializeField] ParticleSystem p_pestA;
    [SerializeField] ParticleSystem p_pestB;
    [SerializeField] ParticleSystem p_pestSpawn;

    private void Start()
    {
        ComputeNextSpawn(gameObject);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SpawnEnemies();
        p_pestSpawn.startLifetime = respawnDelay;
        p_pestSpawn.transform.position = transform.position;
    }

    private void Update()
    {
        if(enemyA != null && enemyB != null)
        {
            link.enabled = true;
            link.SetPosition(0, enemyA.transform.position);
            link.SetPosition(1, Vector3.Lerp(enemyA.transform.position,enemyB.transform.position,0.5f));
            link.SetPosition(2, enemyB.transform.position);

            p_pestA.transform.position = enemyA.transform.position;
            p_pestB.transform.position = enemyB.transform.position;

            if (!p_pestA.isPlaying)
                p_pestA.Play();

            if (!p_pestB.isPlaying)
                p_pestB.Play();
        }

        else if(enemyA != null && enemyB == null)
        {
            p_pestA.transform.position = enemyA.transform.position;
            p_pestB.Stop();
            link.enabled = false;
        }

        else if (enemyB != null && enemyA == null)
        {
            p_pestB.transform.position = enemyB.transform.position;
            p_pestA.Stop();
            link.enabled = false;
        }


        if (enemyA != null && enemyB == null && !isSpawning || enemyA == null && enemyB != null && !isSpawning)
            StartCoroutine(RespawnEnemy());

        if (enemyA == null && enemyB == null)
            Destroy(gameObject);
    }

    private IEnumerator RespawnEnemy ()
    {
        isSpawning = true;

        if (enemyA == null)
            ComputeNextSpawn(enemyB);

        else if (enemyB == null)
            ComputeNextSpawn(enemyA);

        p_pestSpawn.transform.position = nextSpawnPos;
        p_pestSpawn.Play();
        yield return new WaitForSeconds(respawnDelay);
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyA == null)
        {
            enemyA = Instantiate(prefabEnemyA, transform);
            enemyA.transform.position = nextSpawnPos;
        }

        if (enemyB == null)
        {
            enemyB = Instantiate(prefabEnemyB, transform);
            enemyB.transform.position = nextSpawnPos;
        }

        isSpawning = false;
    }

    void Die()
    {
        if (spawner != null)
        {
            spawner.enemyRemaining -= 1;

            if (gameManager.currentEvent != null)
            {
                if (gameManager.currentEvent.currentType == Events.EventsType.killGlobal)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killDuet)
                    gameManager.currentEvent.amountLeft += 1;
            }
        }

        Destroy(gameObject);
    }

    void ComputeNextSpawn(GameObject origin)
    {
        Vector3 position = Random.insideUnitSphere.normalized * spawnSpacing + origin.transform.position;
        nextSpawnPos = new Vector3(position.x, transform.position.y, position.z);
    }
}
