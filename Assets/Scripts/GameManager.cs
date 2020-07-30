using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Players prefabs")]
    public GameObject player0prefab;
    public GameObject player1prefab;

    [Header("Camera container")]
    public Transform camContainer;

    [Header("Score and combos")]
    public int score1;
    public int score2;
    public int combo1;
    public int combo2;

    [Header("Players scripts")]
    public PlayerController player0;
    public PlayerController player1;

    [Header("Players color")]
    public Color player1Color;
    public Color player2Color;

    [Header("Player distance")]
    public bool playerOutRange = false;
    [SerializeField] float maxDistance = 27f;

    [Header("Collectible")]
    [SerializeField] GameObject collectible;
    [SerializeField] GameObject[] items;

    [Header("Respawn")]
    public Vector3 currentProgressionCp;
    [SerializeField] float respawnTime = 15f;
    public float respawnStamp1 = 15f;
    public float respawnStamp2 = 15f;

    private PlayerInputManager inputManager;
    private UIManagement uiManagement;
    private PlayerAssignement playerAssignement;

    public Events currentEvent = null;
    public bool p1HasTakenDamage = false;
    public bool p2HasTakenDamage = false;
    public bool p1HasHealed = false;
    public bool p2HasHealed = false;
    public bool p1IsDead = false;
    public bool p2IsDead = false;

    private void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();
        playerAssignement = gameObject.GetComponent<PlayerAssignement>();
        playerAssignement.SpawnPlayers(inputManager);
    }

    private void Start()
    {
        uiManagement = GameObject.Find("-UI Canvas").GetComponent<UIManagement>();
    }

    private void Update()
    {
        if (inputManager.playerCount == 0 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player0prefab;

        if (inputManager.playerCount == 1 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player1prefab;

        if (inputManager.playerCount == 2 && inputManager.joiningEnabled)
            inputManager.DisableJoining();


        DistanceCheck();
    }

    public void TakeDamage1()
    {
        uiManagement.Damage1();
        p1HasTakenDamage = true;
    }
    public void TakeDamage2()
    {
        uiManagement.Damage2();
        p2HasTakenDamage = true;
    }

    public void GainHP1()
    {
        uiManagement.Heal1();
        p1HasHealed = true;
    }
    public void GainHP2()
    {
        uiManagement.Heal2();
        p2HasHealed = true;
    }

    public void Scoring1(int amount)
    {
        uiManagement.ScorePlus1();
        score1 += amount;
    }
    public void Scoring2(int amount)
    {
        uiManagement.ScorePlus2();
        score2 += amount;
    }

    public void GetThroughSpawner()
    {
        StopCoroutine(Respawn(player0));
        StopCoroutine(Respawn(player1));

        if (player0 != null)
        {
            player0.GainHP(player0.maxHp);
            player0.gameObject.SetActive(true);
        }

        if(player1 != null)
        {
            player1.GainHP(player0.maxHp);
            player1.gameObject.SetActive(true);
        }
    }

    public IEnumerator AddCollectible(int collectibleAmount, Transform origin)
    {
        for (int i = 0; i < collectibleAmount; i++)
        {
            Vector3 position = Random.insideUnitSphere * 2 + origin.position;
            position = new Vector3(position.x, origin.position.y, position.z);
            Instantiate(collectible, position, Quaternion.identity);
            yield return new WaitForEndOfFrame();
        }
    }

    public void AddItems(Transform origin, float dropRate)
    {
        float randomPick = Random.Range(0f, 1f);

        if (randomPick <= dropRate)
        {
            Vector3 position = Random.insideUnitSphere * 2 + origin.position;
            position = new Vector3(position.x, origin.position.y, position.z);
            Instantiate(items[Random.Range(0,items.Length)], position, Quaternion.identity);
        }
    }

    public void Respawn1() => StartCoroutine(Respawn(player0));
    public void Respawn2() => StartCoroutine(Respawn(player1));

    private IEnumerator Respawn(PlayerController playerToRespawn)
    {
        if (player0 != null && player1 != null)
        {
            if (playerToRespawn == player0)
            {
                respawnStamp1 = respawnTime;

                while (respawnStamp1 > 0)
                {
                    respawnStamp1 -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                playerToRespawn.gameObject.SetActive(true);
                respawnStamp1 = 0;
            }

            else
            {
                respawnStamp2 = respawnTime;

                while (respawnStamp2 > 0)
                {
                    respawnStamp2 -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                playerToRespawn.gameObject.SetActive(true);
                respawnStamp2 = 0;
            }
        }

        else
            Restart();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DistanceCheck()
    {
        if (player0 != null && player1 != null && player0.gameObject.activeSelf && player1.gameObject.activeSelf)
        {
            if(Vector3.Distance(player0.transform.position, player1.transform.position) < maxDistance)
                playerOutRange = false;
            else
                playerOutRange = true;
        }

        else
            playerOutRange = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentProgressionCp, 0.4f);
    }
}
