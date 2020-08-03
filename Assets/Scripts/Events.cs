using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public enum EventsType
    {
        killGlobal,
        killMelee,
        killCasterLinear,
        killCasterRadial,
        killDuet,
        pickUpItems,
        dontDie,
        dontTakeDamage,
    }

    public EventsType currentType;

    [Header("Amount of entities")]
    [SerializeField] int amountMax;
    public int amountLeft;

    [Header("Time attributes")]
    [SerializeField] float timeMax;
    public float currentTime;

    private GameManager gameManager;
    private UIManagement uIManagement;
    private bool eventCleared = false;
    private bool eventMissed = false;

    private void Start()
    {
        currentTime = timeMax;
        amountLeft = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.currentEvent = this;
        uIManagement = GameObject.Find("-UI Canvas").GetComponent<UIManagement>();
        uIManagement.eventBar.SetActive(true);
        uIManagement.eventBar.GetComponent<AudioSource>().Play();
        gameManager.p1IsDead = false;
        gameManager.p2IsDead = false;
        gameManager.p1HasTakenDamage = false;
        gameManager.p2HasTakenDamage = false;
    }

    private void Update()
    {
        if (!eventCleared && !eventMissed)
        {
            uIManagement.eventTimer.text = currentTime.ToString("00");

            if (currentTime > 0)
                currentTime -= Time.deltaTime;

            switch (currentType)
            {
                case EventsType.killGlobal:
                    Kill("enemies");
                    break;

                case EventsType.killMelee:
                    Kill("melee enemies");
                    break;

                case EventsType.killCasterLinear:
                    Kill("linear caster enemies");
                    break;

                case EventsType.killCasterRadial:
                    Kill("radial caster enemies");
                    break;

                case EventsType.killDuet:
                    Kill("duet enemies");
                    break;

                case EventsType.pickUpItems:
                    PickUpItems();
                    break;

                case EventsType.dontDie:
                    DontDie();
                    break;

                case EventsType.dontTakeDamage:
                    DontTakeDamage();
                    break;
            }
        }
    }

    private void Kill(string enemyName)
    {
        uIManagement.eventObjective.text = "Kill " + amountMax.ToString() + " " + enemyName +" !";
        uIManagement.eventAmount.text = amountLeft.ToString() + "/" + amountMax.ToString();

        if (amountLeft >= amountMax)
            StartCoroutine(EventCleared());

        else if (amountLeft < amountMax && currentTime <= 0)
            StartCoroutine(EventMissed());
    }

    private void DontDie()
    {
        uIManagement.eventObjective.text = "Don't die !";
        uIManagement.eventAmount.text = "";

        if (gameManager.p1IsDead || gameManager.p2IsDead)
            StartCoroutine(EventMissed());

        else if (!gameManager.p1IsDead && !gameManager.p2IsDead && currentTime <= 0)
            StartCoroutine(EventCleared());
    }

    private void DontTakeDamage()
    {
        uIManagement.eventObjective.text = "Don't get hit !";
        uIManagement.eventAmount.text = "";

        if (gameManager.p1HasTakenDamage || gameManager.p1HasTakenDamage)
            StartCoroutine(EventMissed());

        else if (!gameManager.p1HasTakenDamage && !gameManager.p1HasTakenDamage && currentTime <= 0)
            StartCoroutine(EventCleared());
    }

    private void PickUpItems()
    {
        uIManagement.eventObjective.text = "Pick up " + amountMax.ToString() + " gears !";
        uIManagement.eventAmount.text = amountLeft.ToString() + "/" + amountMax.ToString();

        if (amountLeft >= amountMax)
            StartCoroutine(EventCleared());

        else if (amountLeft < amountMax && currentTime <= 0)
            StartCoroutine(EventMissed());
    }

    private IEnumerator EventCleared()
    {
        eventCleared = true;
        uIManagement.eventBar.GetComponent<Animator>().SetTrigger("EventCleared");
        yield return new WaitForEndOfFrame();
        uIManagement.eventClearedAudio.Play();
        yield return new WaitForSeconds(1f);
        uIManagement.eventBar.SetActive(false);
        this.enabled = false;
    }

    private IEnumerator EventMissed()
    {
        eventMissed = true;
        uIManagement.eventBar.GetComponent<Animator>().SetTrigger("EventMissed");
        yield return new WaitForEndOfFrame();
        uIManagement.eventMissedAudio.Play();
        yield return new WaitForSeconds(1f);
        uIManagement.eventBar.SetActive(false);
        this.enabled = false;
    }
}
