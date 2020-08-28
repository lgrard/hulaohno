using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public float audioVolume;
    public float musicVolume;
    [SerializeField] AudioSource musicManager;

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
    private int globalScore;
    [SerializeField] float comboStampMax = 10f;
    private float comboStamp1;
    private float comboStamp2;
    private int periodScore1 = 0;
    private int periodScore2 = 0;

    [Header("Players scripts")]
    public PlayerController player0;
    public PlayerController player1;

    [Header("Players color")]
    public Color player1Color;
    public Color player2Color;

    [Header("Player distance")]
    public bool playerOutRange = false;
    [SerializeField] float maxDistance = 27f;
    private float distance;
    public float distanceRatio;

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
    public CheckPointTrigger currentTrigger = null;
    public bool p1HasTakenDamage = false;
    public bool p2HasTakenDamage = false;
    public bool p1HasHealed = false;
    public bool p2HasHealed = false;
    public bool p1IsDead = false;
    public bool p2IsDead = false;

    public bool isPaused = false;
    public bool isEnded = false;
    public bool isLoading = false;

    //Init
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("globalScore"))
            PlayerPrefs.SetInt("globalScore", 0);

        if (!PlayerPrefs.HasKey("audioVolumePref"))
            PlayerPrefs.SetFloat("audioVolumePref", 1);

        if (!PlayerPrefs.HasKey("musicVolumePref"))
            PlayerPrefs.SetFloat("musicVolumePref", 1);

        globalScore = PlayerPrefs.GetInt("globalScore");
        audioVolume = PlayerPrefs.GetFloat("audioVolumePref");
        musicVolume = PlayerPrefs.GetFloat("musicVolumePref");

        inputManager = GetComponent<PlayerInputManager>();
        playerAssignement = gameObject.GetComponent<PlayerAssignement>();
        playerAssignement.SpawnPlayers(inputManager);
    }
    private void Start()
    {
        uiManagement = GameObject.Find("-UI Canvas").GetComponent<UIManagement>();
    }

    //Unity Cycle
    private void Update()
    {
        score1 = Mathf.Clamp(score1, 0, 100000000);
        score2 = Mathf.Clamp(score2, 0, 100000000);

        if (inputManager.playerCount == 0 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player0prefab;

        if (inputManager.playerCount == 1 && inputManager.joiningEnabled)
            inputManager.playerPrefab = player1prefab;

        if (inputManager.playerCount == 2 && inputManager.joiningEnabled)
            inputManager.DisableJoining();

        ComboManagement();
        DistanceCheck();
        AudioManagement();

        if (isLoading || isEnded || isPaused)
            Time.timeScale = 0;
        else if(!isLoading && !isEnded && !isPaused)
            Time.timeScale = 1;
    }

    #region Dual methods
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

    public void Scoring1(int amount, bool combo)
    {
        int scoreAmountToAdd = 0;

        if (combo)
        {
            combo1 += 1;
            comboStamp1 = comboStampMax;
            uiManagement.ComboPlus1();
        }

        if (combo1 == 0)
            scoreAmountToAdd = amount;
        else
            scoreAmountToAdd = amount * combo1;

        score1 += scoreAmountToAdd;
        periodScore1 += scoreAmountToAdd;
        globalScore += scoreAmountToAdd;

        uiManagement.ScorePlus1(scoreAmountToAdd);
    }
    public void Scoring2(int amount, bool combo)
    {
        int scoreAmountToAdd = 0;

        if (combo)
        {
            combo2 += 1;
            comboStamp2 = comboStampMax;
            uiManagement.ComboPlus2();
        }

        if (combo1 == 0)
            scoreAmountToAdd = amount;
        else
            scoreAmountToAdd = amount * combo2;

        score2 += scoreAmountToAdd;
        periodScore2 += scoreAmountToAdd;
        globalScore += scoreAmountToAdd;

        uiManagement.ScorePlus2(scoreAmountToAdd);
    }

    public void Respawn1() => StartCoroutine(Respawn(player0));
    public void Respawn2() => StartCoroutine(Respawn(player1));
    #endregion

    private IEnumerator Respawn(PlayerController playerToRespawn)
    {
        if (player0 != null && player1 != null)
        {
            if (playerToRespawn == player0 && !p2IsDead)
            {
                p1IsDead = true;
                respawnStamp1 = respawnTime;

                while (respawnStamp1 > 0)
                {
                    respawnStamp1 -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                playerToRespawn.gameObject.SetActive(true);
                playerToRespawn.Spawn();
                respawnStamp1 = 0;
                p1IsDead = false;
            }

            else if(playerToRespawn == player1 && !p1IsDead)
            {
                p2IsDead = true;
                respawnStamp2 = respawnTime;

                while (respawnStamp2 > 0)
                {
                    respawnStamp2 -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                playerToRespawn.gameObject.SetActive(true);
                playerToRespawn.Spawn();
                respawnStamp2 = 0;
                p2IsDead = false;
            }

            else if (playerToRespawn == player1 && p1IsDead || playerToRespawn == player0 && p2IsDead)
            {
                p1IsDead = true;
                p2IsDead = true;

                score1 -= periodScore1;
                score2 -= periodScore2;
                periodScore1 = 0;
                periodScore2 = 0;

                if (currentTrigger != null)
                    StartCoroutine(currentTrigger.ResetTrigger());

                yield return new WaitForEndOfFrame();
                player0.gameObject.SetActive(true);
                player0.Spawn();

                player1.gameObject.SetActive(true);
                player1.Spawn();

                respawnStamp1 = 0;
                p1IsDead = false;

                respawnStamp2 = 0;
                p2IsDead = false;
            }
        }

        else if(playerToRespawn == player0 && player1 == null)
        {
            yield return new WaitForEndOfFrame();

            if (currentTrigger != null)
                StartCoroutine(currentTrigger.ResetTrigger());

            score1 -= periodScore1;
            periodScore1 = 0;

            p1IsDead = true;
            playerToRespawn.gameObject.SetActive(true);
            p1IsDead = false;
            playerToRespawn.Spawn();
        }

        playerAssignement.RepawnPlayers(playerToRespawn.playerInput, playerToRespawn.playerIndex);
    }

    public void GetThroughSpawner(int amount)
    {
        StopCoroutine(Respawn(player1));
        StopCoroutine(Respawn(player0));

        currentTrigger = null;
        currentEvent = null;

        if (player0 != null)
        {
            player0.GainHP(player0.maxHp);
            player0.gameObject.SetActive(true);
            Scoring1(amount,false);
            periodScore1 = 0;
        }

        if (player1 != null)
        {
            player1.GainHP(player0.maxHp);
            player1.gameObject.SetActive(true);
            Scoring2(amount, false);
            periodScore1 = 0;
        }

        PlayerPrefs.SetInt("globalScore", globalScore);
    }

    #region Add items and collectibles methods
    public IEnumerator AddCollectible(int collectibleAmount, Transform origin, float dispertion)
    {
        for (int i = 0; i < collectibleAmount; i++)
        {
            Vector3 position = Random.insideUnitSphere * dispertion + origin.position;
            position = new Vector3(position.x, origin.position.y, position.z);
            Instantiate(collectible, position, Quaternion.identity);
            yield return new WaitForEndOfFrame();
        }
    }

    public void AddItems(Transform origin, float dropRate, float dispertion)
    {
        float randomPick = Random.Range(0f, 1f);

        if (randomPick <= dropRate)
        {
            Vector3 position = Random.insideUnitSphere * dispertion + origin.position;
            position = new Vector3(position.x, origin.position.y, position.z);
            Instantiate(items[Random.Range(0,items.Length)], position, Quaternion.identity);
        }
    }

    public IEnumerator AddSpecificItems(Transform origin, GameObject[] itemToAdd, float dispertion)
    {
        if(itemToAdd.Length > 0)
        {
            foreach(GameObject item in itemToAdd)
            {
                Vector3 position = Random.insideUnitSphere * dispertion + origin.position;
                position = new Vector3(position.x, origin.position.y, position.z);
                Instantiate(item, position, Quaternion.identity);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    #endregion

    public void Restart()
    {
        uiManagement.OnRestart();
    }

    private void DistanceCheck()
    {
        if (player0 != null && player1 != null && player0.gameObject.activeSelf && player1.gameObject.activeSelf)
        {
            distance = Vector3.Distance(player0.transform.position, player1.transform.position);
            distanceRatio = distance / maxDistance;

            if (distance < maxDistance)
                playerOutRange = false;
            else
                playerOutRange = true;
        }

        else
            playerOutRange = false;
    }

    private void ComboManagement()
    {
        if (combo1 > 0 && comboStamp1 > 0)
            comboStamp1 -= Time.deltaTime;

        else if (combo1 > 0 && comboStamp1 <= 0)
            combo1 = 0;

        if (combo2 > 0 && comboStamp2 > 0)
            comboStamp2 -= Time.deltaTime;

        else if (combo2 > 0 && comboStamp2 <= 0)
            combo2 = 0;
    }

    public void PauseGame()
    {
        if (!isEnded && !isLoading)
        {
            isPaused = !isPaused;
            if(isPaused)
                uiManagement.OpenMenu();
        }
    }

    public void LevelEnd()
    {
        if (!isEnded)
        {
            if(player0!=null)
                player0.enabled = false;
            if (player1 != null)
                player1.enabled = false;

            periodScore1 = 0;
            periodScore2 = 0;
            PlayerPrefs.SetInt("globalScore", globalScore);

            if(PlayerPrefs.GetInt(("hs" + SceneManager.GetActiveScene().buildIndex.ToString())) < score1+score2)
                PlayerPrefs.SetInt("hs" + SceneManager.GetActiveScene().buildIndex.ToString(), score1 + score2);

            isEnded = true;

            uiManagement.OpenWin();
        }
    }

    private void AudioManagement()
    {
        AudioListener.volume = audioVolume;
        musicManager.volume = musicVolume;

        PlayerPrefs.SetFloat("audioVolumePref", audioVolume);
        PlayerPrefs.SetFloat("musicVolumePref", musicVolume);
    }

    //Draw various things on Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentProgressionCp, 0.4f);
    }
}
