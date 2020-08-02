using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;


public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject menu1;
    [SerializeField] GameObject playerAssignementMenu;

    [Header("Settings")]
    [SerializeField] Slider audioSlider;
    [SerializeField] Slider musicSlider;
    private float audioVolume;
    private float musicVolume;
    [SerializeField] AudioSource musicManager;

    [Header("Quit tips")]
    [SerializeField] GameObject quitTips;

    [SerializeField] EventSystem eventSystem;
    private InputSystemUIInputModule eventSystemInput;
    private Button quitButton;

    [Header("Effects")]
    [SerializeField] ParticleSystem p_starsBlue;
    [SerializeField] ParticleSystem p_starsRed;
    [SerializeField] ParticleSystem p_light;

    PlayerAssignement playerAssignement;
    Animator anim;

    //Initilialize
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("audioVolumePref"))
            PlayerPrefs.SetFloat("audioVolumePref", 1);

        if (!PlayerPrefs.HasKey("musicVolumePref"))
            PlayerPrefs.SetFloat("musicVolumePref", 1);

        audioVolume = PlayerPrefs.GetFloat("audioVolumePref");
        musicVolume = PlayerPrefs.GetFloat("musicVolumePref");

        audioSlider.value = audioVolume;
        musicSlider.value = musicVolume;
    }
    private void Start()
    {
        Time.timeScale = 1f;
        anim = gameObject.GetComponent<Animator>();
        playerAssignement = gameObject.GetComponent<PlayerAssignement>();
        eventSystemInput = eventSystem.gameObject.GetComponent<InputSystemUIInputModule>();
        eventSystem.firstSelectedGameObject.GetComponent<Button>().OnSelect(null);
    }

    //Unity cycles
    private void Update()
    {
        PlayerAssignementMenu();
        AudioManagement();
    }

    private void PlayerAssignementMenu()
    {
        if (playerAssignementMenu.activeSelf)
        {
            anim.SetBool("p1Paired", playerAssignement.device0Paired);
            anim.SetBool("p2Paired", playerAssignement.device1Paired);

            if (playerAssignement.device0Paired && !p_starsBlue.isPlaying)
                p_starsBlue.Play();

            if (playerAssignement.device1Paired && !p_starsRed.isPlaying)
                p_starsRed.Play();

            if (playerAssignement.device0Paired && playerAssignement.device1Paired && !p_light.isPlaying)
                p_light.Play();
        }
    }
    private void AudioManagement()
    {
        AudioListener.volume = audioVolume;
        musicManager.volume = musicVolume;

        PlayerPrefs.SetFloat("audioVolumePref", audioVolume);
        PlayerPrefs.SetFloat("musicVolumePref", musicVolume);

        audioVolume = audioSlider.value;
        musicVolume = musicSlider.value;
    }

    public void OnQuit()
    {
        quitButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();

        if (!quitTips.activeSelf)
        {
            quitTips.SetActive(true);
            eventSystemInput.submit.action.performed += Quit;
            eventSystemInput.cancel.action.performed += CancelQuit;
            eventSystem.SetSelectedGameObject(null);
        }
    }
    public void OnAdventure() => StartCoroutine(AdventureTransition());
    public void OnCredits()
    {
        Debug.Log("insèrer le nom de la fine équipe");
    }
    private void OnCancelAssignement(CallbackContext ctx)
    {
        if(playerAssignementMenu.activeSelf && !playerAssignement.device0Paired && !playerAssignement.device1Paired)
            StartCoroutine(PlayerAssignementTransition());
    }

    private IEnumerator AdventureTransition()
    {
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(0.5f);
        menu1.SetActive(false);
        playerAssignementMenu.SetActive(true);
        playerAssignement.enabled = true;
        eventSystemInput.cancel.action.performed += OnCancelAssignement;
    }
    private IEnumerator PlayerAssignementTransition()
    {
        eventSystemInput.cancel.action.performed -= OnCancelAssignement;
        playerAssignement.enabled = false;
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(0.5f);
        playerAssignementMenu.SetActive(false);
        menu1.SetActive(true);
    }

    private void CancelQuit(CallbackContext ctx)
    {
        eventSystem.SetSelectedGameObject(quitButton.gameObject);
        eventSystem.currentSelectedGameObject.GetComponent<Button>().OnSelect(null);
        quitTips.SetActive(false);
        eventSystemInput.submit.action.performed -= Quit;
        eventSystemInput.cancel.action.performed -= CancelQuit;
    }
    private void Quit(CallbackContext ctx) => Application.Quit();
}
