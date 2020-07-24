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

    private UIManagement uIManagement;
    private bool eventCleared;

    private void Start()
    {
        currentTime = timeMax;
        amountLeft = 0;
        uIManagement = GameObject.Find("-UI Canvas").GetComponent<UIManagement>();
        uIManagement.eventBar.SetActive(true);
    }

    private void Update()
    {
        if (!eventCleared)
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

                case EventsType.pickUpItems:

                    break;

                case EventsType.dontDie:

                    break;

                case EventsType.dontTakeDamage:

                    break;
            }
        }

        else
        {
            uIManagement.eventBar.SetActive(false);
            this.enabled = false;
        }
    }

    private void Kill(string enemyName)
    {
        uIManagement.eventObjective.text = "Kill " + amountMax.ToString() + " " + enemyName;
        uIManagement.eventAmount.text = amountLeft.ToString() + "/" + amountMax.ToString();

        if (amountLeft >= amountMax)
            eventCleared = true;
    }
}
