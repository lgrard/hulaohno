using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

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

    [Header("Collectible")]
    [SerializeField] GameObject collectible;
    [SerializeField] GameObject[] items;

    [Header("Respawn")]
    public Vector3 currentProgressionCp;

    private PlayerInputManager inputManager;
    private UIManagement uiManagement;

    public Events currentEvent = null;
    public bool p1HasTakenDamage = false;
    public bool p2HasTakenDamage = false;
    public bool p1HasHealed = false;
    public bool p2HasHealed = false;
    public bool p1IsDead = false;
    public bool p2IsDead = false;


    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
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
        if(player0 != null)
            player0.GainHP(player0.maxHp);
        if(player1 != null)
            player1.GainHP(player1.maxHp);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentProgressionCp, 0.4f);
    }
}
