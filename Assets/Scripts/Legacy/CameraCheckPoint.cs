﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CameraCheckPoint : MonoBehaviour
{
    [Header("Camera position and rotation")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    [Header("Trigger zone position and size")]
    public Vector3 zonePosition;
    public Vector3 zoneSize;
    public Vector3 triggerSize;

    [Header("Does this zone uses blocks the players")]
    public bool blocksPlayers;

    [Header("Does this zone uses blocks the players")]
    public bool controlsCam;

    [Header("Linked Spawner (null if blocksPlayers = false)")]
    public List<EnemySpawner> linkedSpawner = new List<EnemySpawner>();
    private List<EnemySpawner> endedSpawner = new List<EnemySpawner>();

    public GameObject wallL;
    public GameObject wallR;

    public float triggerOffset = 0;

    public bool automaticWall;
    bool blockTriggered = false;
    bool playerInsideTrigger;
    bool playerInside;
    float timeStamp = 1;
    int playerLayer;
    Camera cam;
    Events linkedEvents;
    GameManager gameManager;



    //Initialize
    private void Start()
    {
        wallL.SetActive(false);
        wallR.SetActive(false);

        if (TryGetComponent(out Events events))
        {
            linkedEvents = events;
            linkedEvents.enabled = false;
        }

        playerLayer = LayerMask.GetMask("Player");
        cam = Camera.main;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (EnemySpawner spawner in linkedSpawner)
        {
            spawner.isSpawning = true;
            spawner.enabled = false;

            if(linkedEvents != null)
                spawner.linkedCameraCheckpoint = gameObject;
        }
    }

    //Update method
    private void Update()
    {
        if (blocksPlayers)
            triggerOffset = Mathf.Clamp(triggerOffset, 6, 10000);

        triggerSize = new Vector3(zoneSize.x - triggerOffset, zoneSize.y, zoneSize.z - triggerOffset);

        if (automaticWall)
        {
            wallL.transform.localScale = new Vector3(1,10,zoneSize.z);
            wallL.transform.localPosition = new Vector3(zoneSize.x / 2 + wallL.transform.localScale.x/2, 2, 0);
            wallL.transform.rotation = Quaternion.Euler(Vector3.zero);

            wallR.transform.localScale = new Vector3(2, 10, zoneSize.z);
            wallR.transform.localPosition = new Vector3(-zoneSize.x / 2 - wallL.transform.localScale.x / 2, 2, 0);
            wallR.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        gameObject.transform.position = zonePosition;
    }

    //Fixed Update method
    private void FixedUpdate()
    {
        Blocking();

        playerInsideTrigger = Physics.CheckBox(zonePosition, triggerSize/ 2,Quaternion.identity,playerLayer,QueryTriggerInteraction.Ignore);
        playerInside = Physics.CheckBox(zonePosition, zoneSize / 2, Quaternion.identity, playerLayer, QueryTriggerInteraction.Ignore);

        if (blockTriggered && playerInside && controlsCam)
        {
            cam.GetComponentInParent<CameraEffects>().checkPointActive = true;
            Transform camContainer = cam.GetComponentInParent<CameraEffects>().transform;
            camContainer.position = Vector3.Lerp(cam.transform.position, cameraPosition, 0.05f);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(cameraRotation), 0.05f);
        }

        else
            cam.GetComponentInParent<CameraEffects>().checkPointActive = false;
    }

    //Blocking method
    private void Blocking()
    {
        if (playerInsideTrigger && !blockTriggered)
        {
            Collider[] colliders = Physics.OverlapBox(zonePosition, zoneSize / 2, Quaternion.identity, playerLayer, QueryTriggerInteraction.Ignore);
            if(colliders.Length == 1 && gameManager.player0 != null && gameManager.player1 != null)
            {
                if (colliders[0].gameObject.GetComponent<PlayerController>().playerIndex == 0)
                    gameManager.player1.transform.position = gameManager.player0.playerSpawnPoint.position;

                else if (colliders[0].gameObject.GetComponent<PlayerController>().playerIndex == 1)
                    gameManager.player0.transform.position = gameManager.player1.playerSpawnPoint.position;
            }

            blockTriggered = true;

            if (blocksPlayers)
            {
                wallL.SetActive(true);
                wallR.SetActive(true);
            }

            foreach (EnemySpawner spawner in linkedSpawner)
                spawner.enabled = true;

            if (linkedEvents != null)
                linkedEvents.enabled = true;
        }

        if (linkedSpawner.Count > 0 && linkedSpawner.Count > endedSpawner.Count)
        {
            foreach (EnemySpawner spawner in linkedSpawner)
            {
                if (spawner.enemyRemaining == 0 && !spawner.noMoreEnemies)
                {
                    endedSpawner.Add(spawner);
                    spawner.noMoreEnemies = true;
                }
            }
        }

        else if (blocksPlayers && linkedSpawner.Count == endedSpawner.Count)
        {
            wallL.GetComponent<Animator>().SetTrigger("Out");
            wallR.GetComponent<Animator>().SetTrigger("Out");

            if(timeStamp > 0)
                timeStamp -= Time.deltaTime;

            else
            {
                wallL.SetActive(false);
                wallR.SetActive(false);
            }
        }
    }
 }
