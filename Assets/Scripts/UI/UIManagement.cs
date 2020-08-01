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

    [Header("HP bar")]
    [SerializeField] GameObject p1HPBar;
    [SerializeField] GameObject p2HPBar;
    [SerializeField] RectTransform waveHp1;
    [SerializeField] RectTransform waveHp2;
    [SerializeField] Text respawnTimer1;
    [SerializeField] Text respawnTimer2;

    [Header("Score counter")]
    [SerializeField] Text scoreCounter1;
    [SerializeField] Text scoreCounter2;

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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSlider.value = gameManager.audioVolume;
        musicSlider.value = gameManager.musicVolume;

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
            BarManagement(player1, p1HPBar, waveHp1, gameManager.score1, scoreCounter1,respawnTimer1, gameManager.respawnStamp1, gameManager.combo1);
        else
            p1HPBar.SetActive(false);

        if (player2 != null)
            BarManagement(player2, p2HPBar, waveHp2, gameManager.score2, scoreCounter2, respawnTimer2, gameManager.respawnStamp2, gameManager.combo2);
        else
            p2HPBar.SetActive(false);
    }

    //Play an animation when score increases
    public void ScorePlus1() => p1HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");
    public void ScorePlus2() => p2HPBar.GetComponent<Animator>().SetTrigger("ScorePlus");

    //Play an animation when combo increases
    public void ComboPlus1() => p1HPBar.GetComponent<Animator>().SetTrigger("ComboPlus");
    public void ComboPlus2() => p2HPBar.GetComponent<Animator>().SetTrigger("ComboPlus");

    //Play an animation when takes damage
    public void Damage1() => p1HPBar.GetComponent<Animator>().SetTrigger("TakesDamage");
    public void Damage2() => p2HPBar.GetComponent<Animator>().SetTrigger("TakesDamage");

    public void Heal1() => p1HPBar.GetComponent<Animator>().SetTrigger("Heal");
    public void Heal2() => p2HPBar.GetComponent<Animator>().SetTrigger("Heal");

    //Initialize and manage the HP bar and score display
    private void BarManagement(PlayerController player, GameObject globalBar, RectTransform waveHp, int playerScore, Text scoreContainer, Text respawnTimer, float respawnStamp, int playerCombo)
    {
        //Hp setting
        globalBar.SetActive(true);

        float Hp = player.HP;
        float maxHp = player.maxHp;
        float currentRatio = Hp / maxHp;

        waveHp.localScale = new Vector2(currentRatio, 1);

        //Score setting
        scoreContainer.text = "SCORE - " + playerScore.ToString("000000");
        //Combo setting
        comboCounter1.text = "x" + playerCombo.ToString();

        //Respawn timer setting
        if (Mathf.CeilToInt(respawnStamp) > 0)
        {
            respawnTimer.enabled = true;
            respawnTimer.text = Mathf.CeilToInt(respawnStamp).ToString();
        }

        else
            respawnTimer.enabled = false;

        comboCounter1.enabled = playerCombo >= 1;

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
        gameManager.isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnRestart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void OnNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
}
