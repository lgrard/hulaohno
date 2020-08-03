﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    bool triggered = false;
    bool playerInside1;
    bool playerInside2;
    BoxCollider collider;
    [SerializeField] Vector3 triggerZoneOffset;
    [SerializeField] Vector3 triggerSize;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        collider = gameObject.AddComponent<BoxCollider>();
        collider.center = triggerZoneOffset;
        collider.size = triggerSize;
        collider.isTrigger = true;
    }

    //Unity Cycles : Check if the player is in the checkpoint and end the level
    private void FixedUpdate()
    {
        if (playerInside1 && playerInside2 && !triggered || playerInside1 && !triggered && gameManager.player1 == null)
        {
            triggered = true;
            gameManager.LevelEnd();
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController1) && playerController1.playerIndex == 0 && other.GetType() == typeof(CapsuleCollider))
            playerInside1 = true;

        if (other.TryGetComponent<PlayerController>(out PlayerController playerController2) && playerController2.playerIndex == 1 && other.GetType() == typeof(CapsuleCollider))
            playerInside2 = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController1) && playerController1.playerIndex == 0 && other.GetType() == typeof(CapsuleCollider))
            playerInside1 = false;

        if (other.TryGetComponent<PlayerController>(out PlayerController playerController2) && playerController2.playerIndex == 1 && other.GetType() == typeof(CapsuleCollider))
            playerInside2 = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero + triggerZoneOffset, triggerSize);
    }
}

