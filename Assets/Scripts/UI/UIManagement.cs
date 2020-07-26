using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController player1;
    private PlayerController player2;

    #region old bar
    /*
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


    private GameObject[] p1HPBar;
    private GameObject[] p2HPBar;
    */
    #endregion

    [Header("HP bar")]
    [SerializeField] GameObject p1HPBar;
    [SerializeField] GameObject p2HPBar;
    [SerializeField] RectTransform waveHp1;
    [SerializeField] RectTransform waveHp2;

    [Header("Score counter")]
    [SerializeField] Text scoreCounter1;
    [SerializeField] Text scoreCounter2;

    [Header("Event bar")]
    public GameObject eventBar;
    public Text eventObjective;
    public Text eventTimer;
    public Text eventAmount;
    public AudioSource eventClearedAudio;
    public AudioSource eventMissedAudio;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        #region old bar
        /*
        p1HPBar = new GameObject[] { p1_1, p1_2, p1_3, p1_4, p1_5};
        p2HPBar = new GameObject[] { p2_1, p2_2, p2_3, p2_4, p2_5 };
        */
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        player1 = gameManager.player0;
        player2 = gameManager.player1;

        //Display score and health if player is assigned        

        if (player1 != null)
            BarManagement(player1, p1HPBar, waveHp1, gameManager.score1, scoreCounter1);
        else
            p1HPBar.SetActive(false);

        if (player2 != null)
            BarManagement(player2, p2HPBar, waveHp2, gameManager.score2, scoreCounter2);
        else
            p2HPBar.SetActive(false);
    }

    //Play an animation when score increases
    public void ScorePlus1() => p1HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");
    public void ScorePlus2() => p2HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");

    //Play an animation when takes damage
    public void Damage1() => p1HPBar.GetComponent<Animator>().SetTrigger("TakesDamage");
    public void Damage2() => p2HPBar.GetComponent<Animator>().SetTrigger("TakesDamage");

    public void Heal1() => p1HPBar.GetComponent<Animator>().SetTrigger("Heal");
    public void Heal2() => p2HPBar.GetComponent<Animator>().SetTrigger("Heal");

    //Initialize and manage the HP bar and score display
    private void BarManagement(PlayerController player, GameObject globalBar, RectTransform waveHp, int playerScore, Text scoreContainer)
    {
        //Hp setting
        globalBar.SetActive(true);

        float Hp = player.HP;
        float maxHp = player.maxHp;
        float currentRatio = Hp / maxHp;

        waveHp.localScale = new Vector2(currentRatio, 1);

        //Score setting
        scoreContainer.text = "SCORE - " + playerScore.ToString("000000");

        #region old bar
        /*
        foreach (GameObject bar in pbar)
        {
            int number = int.Parse (bar.name.Substring(bar.name.Length - 1));
            if (number > HP1)
                bar.SetActive(false);

            else
                bar.SetActive(true);
        }
        */
        #endregion
    }
}
