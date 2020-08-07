﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


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

    [Header("Win menu")]
    [SerializeField] GameObject winMenu;
    [SerializeField] Button nextLevelButton;

    [Header("Pause menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject lastSelectedObject;
    [SerializeField] EventSystem eventSystem;

    [Header("Settings")]
    [SerializeField] Slider audioSlider;
    [SerializeField] Slider musicSlider;

    [Header("Loading screen")]
    public LoadingSceneManager loadingSceneManager;
    [SerializeField] Animator transitionAnim;

    [Header("HP bar")]
    [SerializeField] GameObject p1HPBar;
    [SerializeField] GameObject p2HPBar;
    [SerializeField] RectTransform waveHp1;
    [SerializeField] RectTransform waveHp2;
    [SerializeField] Text respawnTimer1;
    [SerializeField] Text respawnTimer2;
    private Animator p1HPBarAnim;
    private Animator p2HPBarAnim;

    [Header("Score counter")]
    [SerializeField] Text scoreCounter1;
    [SerializeField] Text scoreCounter2;
    [SerializeField] Text scorePeriodCounter1;
    [SerializeField] Text scorePeriodCounter2;
    [SerializeField] float scoreStampMax = 1f;
    private float scoreStamp1 = 0;
    private int currentScorePeriod1;
    private float scoreStamp2 = 0;
    private int currentScorePeriod2;

    [Header("Combo counter")]
    [SerializeField] Text comboCounter1;
    [SerializeField] Text comboCounter2;

    [Header("Event bar")]
    public GameObject eventBar;
    public Text eventObjective;
    public Text eventTimer;
    public Text eventAmount;
    public AudioSource eventClearedAudio;
    public AudioSource eventMissedAudio;

    private void Start()
    {
        Time.timeScale = 1f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSlider.value = gameManager.audioVolume;
        musicSlider.value = gameManager.musicVolume;

        p1HPBarAnim = p1HPBar.GetComponent<Animator>();
        p2HPBarAnim = p2HPBar.GetComponent<Animator>();

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
        gameManager.audioVolume = audioSlider.value;
        gameManager.musicVolume = musicSlider.value;

        player1 = gameManager.player0;
        player2 = gameManager.player1;

        pauseMenu.SetActive(gameManager.isPaused);

        if (pauseMenu.activeSelf)
            lastSelectedObject = eventSystem.currentSelectedGameObject;

        //Display score and health if player is assigned        
        if (player1 != null)
            BarManagement(player1, p1HPBar, waveHp1, gameManager.score1, scoreCounter1,respawnTimer1,
            gameManager.respawnStamp1, gameManager.combo1,scorePeriodCounter1,currentScorePeriod1,comboCounter1,scoreStamp1,p1HPBarAnim);
        else
            p1HPBar.SetActive(false);

        if (player2 != null)
            BarManagement(player2, p2HPBar, waveHp2, gameManager.score2, scoreCounter2, respawnTimer2,
            gameManager.respawnStamp2, gameManager.combo2, scorePeriodCounter2, currentScorePeriod2, comboCounter2, scoreStamp2,p2HPBarAnim);
        else
            p2HPBar.SetActive(false);

        ScorePeriodManagement();
    }

    //Play an animation when score increases
    public void ScorePlus1(int amount)
    {
        p1HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");
        scoreStamp1 = scoreStampMax;
        currentScorePeriod1 += amount;
    }
    public void ScorePlus2(int amount)
    {
        p2HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");
        scoreStamp2 = scoreStampMax;
        currentScorePeriod2 += amount;
    }

    //Play an animation when combo increases
    public void ComboPlus1() => p1HPBarAnim.SetTrigger("ComboPlus");
    public void ComboPlus2() => p2HPBarAnim.SetTrigger("ComboPlus");

    //Play an animation when takes damage
    public void Damage1() => p1HPBarAnim.SetTrigger("TakesDamage");
    public void Damage2() => p2HPBarAnim.SetTrigger("TakesDamage");

    public void Heal1() => p1HPBarAnim.SetTrigger("Heal");
    public void Heal2() => p2HPBarAnim.SetTrigger("Heal");

    //Initialize and manage the HP bar and score display
    private void BarManagement(PlayerController player, GameObject globalBar, RectTransform waveHp, int playerScore, Text scoreContainer,
    Text respawnTimer, float respawnStamp, int playerCombo, Text scorePeriodContainer,int currentScorePeriod, Text comboCounter,float scoreStamp, Animator barAnim)
    {
        //Hp setting
        globalBar.SetActive(true);

        float Hp = player.HP;
        float maxHp = player.maxHp;
        float currentRatio = Hp / maxHp;

        waveHp.localScale = new Vector2(currentRatio, 1);

        //Score setting
        scoreContainer.text = "SCORE - " + playerScore.ToString("000000");
        scorePeriodContainer.text = "+ " + currentScorePeriod.ToString("000000");

        barAnim.SetBool("ScorePeriodActive", scoreStamp > 0);

        //Combo setting
        comboCounter.text = "x" + playerCombo.ToString();

        //Respawn timer setting
        if (Mathf.CeilToInt(respawnStamp) > 0)
        {
            respawnTimer.enabled = true;
            respawnTimer.text = Mathf.CeilToInt(respawnStamp).ToString();
        }

        else
            respawnTimer.enabled = false;

        comboCounter.enabled = playerCombo >= 1;

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

    private void ScorePeriodManagement()
    {
        if (scoreStamp1 > 0 && player1 != null)
            scoreStamp1 -= Time.deltaTime;
        else
            currentScorePeriod1 = 0;

        if (scoreStamp2 > 0 && player2 != null)
            scoreStamp2 -= Time.deltaTime;
        else
            currentScorePeriod2 = 0;
    }

    //Open menu methods
    public void OpenMenu()
    {
        eventSystem.SetSelectedGameObject(lastSelectedObject);
        if(lastSelectedObject.TryGetComponent<Button>(out Button button))
            button.OnSelect(null);
        if (lastSelectedObject.TryGetComponent<Slider>(out Slider slider))
            slider.OnSelect(null);
        if (lastSelectedObject.TryGetComponent<Dropdown>(out Dropdown dropDown))
            dropDown.OnSelect(null);
        if (lastSelectedObject.TryGetComponent<Toggle>(out Toggle toggle))
            toggle.OnSelect(null);
    }
    public void OpenWin()
    {
        winMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(nextLevelButton.gameObject);
        nextLevelButton.OnSelect(null);
    }

    //Pause menu methods
    public void OnResume() => gameManager.PauseGame();
    public void OnQuit()
    {
        eventSystem.enabled = false;
        gameManager.isLoading = true;
        transitionAnim.SetTrigger("Transition");
        loadingSceneManager.LoadLevel(0);
    }
    public void OnRestart()
    {
        eventSystem.enabled = false;
        gameManager.isLoading = true;
        transitionAnim.SetTrigger("Transition");
        loadingSceneManager.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnNextLevel()
    {
        eventSystem.enabled = false;
        gameManager.isLoading = true;
        transitionAnim.SetTrigger("Transition");
        loadingSceneManager.LoadLevel(0);
    }
}
