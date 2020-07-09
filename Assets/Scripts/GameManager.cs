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
    public int score;
    public int combo1;
    public int combo2;
    public float specialGauge0 = 0;
    public float specialGauge1 = 0;
    private float specialGaugeGlobal = 0;

    [Header("Players scripts")]
    public PlayerController player0;
    public PlayerController player1;

    [Header("Players color")]
    public Color player1Color;
    public Color player2Color;

    private PlayerInputManager inputManager;
        

    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void Update()
    {
        Special();

        if (inputManager.playerCount == 0 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player0prefab;

        if (inputManager.playerCount == 1 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player1prefab;

        if (inputManager.playerCount == 2 && inputManager.joiningEnabled)
            inputManager.DisableJoining();
    }

    public void Scoring(int amount)
    {
        score += amount;
    }

    private void Special()
    {

    }
}
