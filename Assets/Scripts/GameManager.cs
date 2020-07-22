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
    public GameObject collectible;

    private PlayerInputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
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

    public void Scoring1(int amount) => score1 += amount;
    public void Scoring2(int amount) => score2 += amount;

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

}
