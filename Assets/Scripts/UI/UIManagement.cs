using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController player1;
    private PlayerController player2;

    [Header("P1 HP Bar")]
    [SerializeField] GameObject globalBar1;
    [SerializeField] GameObject p1_1;
    [SerializeField] GameObject p1_2;
    [SerializeField] GameObject p1_3;
    [SerializeField] GameObject p1_4;
    [SerializeField] GameObject p1_5;

    [Header("P2 HP Bar")]
    [SerializeField] GameObject globalBar2;
    [SerializeField] GameObject p2_1;
    [SerializeField] GameObject p2_2;
    [SerializeField] GameObject p2_3;
    [SerializeField] GameObject p2_4;
    [SerializeField] GameObject p2_5;

    [Header("Score counter")]
    [SerializeField] Text scoreCounter1;
    [SerializeField] Text scoreCounter2;


    private GameObject[] p1HPBar;
    private GameObject[] p2HPBar;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        p1HPBar = new GameObject[] { p1_1, p1_2, p1_3, p1_4, p1_5};
        p2HPBar = new GameObject[] { p2_1, p2_2, p2_3, p2_4, p2_5 };
    }

    // Update is called once per frame
    void Update()
    {
        player1 = gameManager.player0;
        player2 = gameManager.player1;

        if (player1 != null) 
            HealthBar(player1, p1HPBar, globalBar1, scoreCounter1);

        else
        {
            globalBar1.SetActive(false);
            scoreCounter1.enabled = false;
        }


        if (player2 != null)
            HealthBar(player2, p2HPBar, globalBar2, scoreCounter2);

        else
        {
            globalBar2.SetActive(false);
            scoreCounter2.enabled = false;
        }


        ScoreSetting(gameManager.score1, scoreCounter1);
        ScoreSetting(gameManager.score2, scoreCounter2);
    }

    private void HealthBar(PlayerController player, GameObject[] pbar, GameObject globalBar, Text scoreCounter)
    {
        scoreCounter.enabled = true;
        globalBar.SetActive(true);

        int HP1 = player.HP;

        foreach (GameObject bar in pbar)
        {
            int number = int.Parse (bar.name.Substring(bar.name.Length - 1));
            if (number > HP1)
                bar.SetActive(false);

            else
                bar.SetActive(true);
        }
    }

    private void ScoreSetting(int playerScore, Text scoreContainer)
    {
        scoreContainer.text = "SCORE - " + playerScore.ToString();
    }
}
