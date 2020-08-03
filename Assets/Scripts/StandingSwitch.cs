﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingSwitch : MonoBehaviour
{
    [SerializeField] Vector3 triggerSize;
    [SerializeField] Vector3 triggerOffset;
    [SerializeField] LayerMask playerLayer;
    bool playerInside;
    bool triggered;

    [SerializeField] InteractionType currentType;
    [SerializeField] GameObject[] linkedObjects;

    enum InteractionType
    {
        openDoor,
        movePlatform,
    }

    private void Awake()
    {
        if (currentType == InteractionType.movePlatform && linkedObjects != null)
            StartCoroutine(InitMovePlat());
    }

    private void Update()
    {
        if (playerInside && !triggered && linkedObjects != null)
        {
            triggered = true;

            switch (currentType)
            {
                case InteractionType.openDoor:
                    StartCoroutine(OpenDoor());
                    break;

                case InteractionType.movePlatform:
                    StartCoroutine(movePlatform());
                    break;
            }
        }

        else if(!playerInside && triggered)
            triggered = false;
    }

    IEnumerator OpenDoor()
    {
        foreach(GameObject linkedObject in linkedObjects)
        {
            while (playerInside)
            {
                linkedObject.GetComponent<SwitchDoor>().isOpenning = true;
                yield return new WaitForEndOfFrame();
            }

            linkedObject.GetComponent<SwitchDoor>().isOpenning = false;
        }
    }
    IEnumerator movePlatform()
    {
        foreach (GameObject linkedObject in linkedObjects)
        {
            while (playerInside)
            {
                linkedObject.GetComponentInChildren<MovingPlatform>().isMoving = true;
                yield return new WaitForEndOfFrame();
            }

            linkedObject.GetComponentInChildren<MovingPlatform>().isMoving = false;
        }
    }

    IEnumerator InitMovePlat()
    {
        foreach (GameObject linkedObject in linkedObjects)
        {
            MovingPlatform movingPlatScript = linkedObject.GetComponentInChildren<MovingPlatform>();
            movingPlatScript.currentType = MovingPlatform.MovementType.switchable;
            linkedObject.GetComponentInChildren<MovingPlatform>().isMoving = false;
            yield return new WaitForEndOfFrame();
        }
    }

    //Checks if a player is in the zone
    private void FixedUpdate() => playerInside = Physics.CheckBox(gameObject.transform.position + triggerOffset, triggerSize / 2, gameObject.transform.rotation, playerLayer, QueryTriggerInteraction.Ignore);

    //Draw various things on gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero + triggerOffset, triggerSize);
    }
}