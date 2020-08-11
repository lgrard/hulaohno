using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CheckPointTrigger : MonoBehaviour
{
    [Header("Camera position and rotation")]
    [SerializeField] Vector3 cameraPositionOffset;
    [SerializeField] Vector3 cameraRotationOffset;

    [Header("Trigger zone position and offset")]
    [SerializeField] Vector3 zonePositionOffset;
    private Vector3 zonePosition;
    [SerializeField] float triggerOffset = 0;

    [Header("Trigger sphere radius")]
    [SerializeField] float zoneRadius;
    private float triggerRadius;

    [Header("Trigger box zone size")]
    [SerializeField] Vector3 zoneSize;
    private Vector3 triggerSize;

    [Header("Does this zone uses blocks the players")]
    [SerializeField] bool blocksPlayers;

    [Header("Does this zone uses blocks the players")]
    [SerializeField] bool controlsCam;

    [Header("Linked Spawner (null if blocksPlayers = false)")]
    [SerializeField] List<EnemySpawner> linkedSpawner = new List<EnemySpawner>();
    private List<EnemySpawner> endedSpawner = new List<EnemySpawner>();

    [Header("Wall objects")]
    [SerializeField] GameObject wallL;
    [SerializeField] GameObject wallR;

    [Header("Zone type")]
    [SerializeField] ZoneType currentType;
    private enum ZoneType
    {
        sphere,
        box,
    }


    [SerializeField] bool automaticWall;
    bool blockTriggered = false;
    bool playerInsideTrigger = false;
    bool playerInside = false;
    bool disableCamBehavior = false;
    bool disableNonBlockingCam = false;
    float timeStamp = 1;
    int playerLayer;
    Camera cam;
    Events linkedEvents;
    GameManager gameManager;

    Collider[] collidersCam = null;




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

            if (linkedEvents != null)
                spawner.linkedCameraCheckpoint = gameObject;
        }
    }

    //Update method
    private void Update()
    {
        transform.localScale = Vector3.one;

        if (blocksPlayers && currentType == ZoneType.box)
            triggerOffset = Mathf.Clamp(triggerOffset, 6, 1000);

        else if (blocksPlayers && currentType == ZoneType.sphere)
            triggerOffset = Mathf.Clamp(triggerOffset, 3, zoneRadius);

        else if (!blocksPlayers)
            triggerOffset = Mathf.Clamp(triggerOffset, 0, 10000);

        triggerRadius = zoneRadius - triggerOffset;
        triggerSize = new Vector3(zoneSize.x - triggerOffset, zoneSize.y, zoneSize.z - triggerOffset);

        if (automaticWall)
        {
            if(currentType == ZoneType.box)
            {
                wallL.transform.localScale = new Vector3(2, 10, zoneSize.z);
                wallL.transform.localPosition = new Vector3(zoneSize.x / 2 + wallL.transform.localScale.x / 2, 2, 0);
                wallL.transform.rotation = gameObject.transform.rotation;

                wallR.transform.localScale = new Vector3(2, 10, zoneSize.z);
                wallR.transform.localPosition = new Vector3(-zoneSize.x / 2 - wallL.transform.localScale.x / 2, 2, 0);
                wallR.transform.rotation = gameObject.transform.rotation;
            }

            if (currentType == ZoneType.sphere)
            {
                wallL.transform.localScale = new Vector3(2, 10, zoneRadius*2);
                wallL.transform.localPosition = new Vector3(zoneRadius + wallL.transform.localScale.x / 2, 2, 0);
                wallL.transform.rotation = gameObject.transform.rotation;

                wallR.transform.localScale = new Vector3(2, 10, zoneRadius * 2);
                wallR.transform.localPosition = new Vector3(-zoneRadius - wallL.transform.localScale.x / 2, 2, 0);
                wallR.transform.rotation = gameObject.transform.rotation;
            }
        }

        zonePosition = gameObject.transform.position + zonePositionOffset;
    }

    //Fixed Update method
    private void FixedUpdate()
    {
        if(currentType == ZoneType.box)
        {
            playerInsideTrigger = Physics.CheckBox(zonePosition, triggerSize / 2, gameObject.transform.rotation, playerLayer, QueryTriggerInteraction.Ignore);
            playerInside = Physics.CheckBox(zonePosition, zoneSize / 2, gameObject.transform.rotation, playerLayer, QueryTriggerInteraction.Ignore);
        }

        else if(currentType == ZoneType.sphere)
        {
            playerInsideTrigger = Physics.CheckSphere(zonePosition, triggerRadius, playerLayer, QueryTriggerInteraction.Ignore);
            playerInside = Physics.CheckSphere(zonePosition, zoneRadius, playerLayer, QueryTriggerInteraction.Ignore);
        }

        if (blockTriggered && playerInside && controlsCam && !disableNonBlockingCam)
        {
            cam.GetComponentInParent<CameraEffects>().checkPointActive = true;
            disableCamBehavior = true;
            Transform camContainer = cam.GetComponentInParent<CameraEffects>().transform;
            camContainer.position = Vector3.Lerp(cam.transform.position, gameObject.transform.position + cameraPositionOffset, 0.05f);
            camContainer.rotation = Quaternion.Slerp(camContainer.rotation, Quaternion.Euler(transform.rotation.eulerAngles + cameraRotationOffset), 0.05f);
        }

        else if (disableCamBehavior && !playerInside && blockTriggered || disableNonBlockingCam && disableCamBehavior)
        {
            cam.GetComponentInParent<CameraEffects>().checkPointActive = false;
            disableCamBehavior = false;
        }

        Blocking();
        NonBlockingCam();
    }

    //Blocking method
    private void Blocking()
    {
        if (playerInsideTrigger && !blockTriggered)
        {
            Collider[] colliders = null;
            
            if(currentType == ZoneType.box)
                colliders = Physics.OverlapBox(zonePosition, zoneSize / 2, gameObject.transform.rotation, playerLayer, QueryTriggerInteraction.Ignore);

            if (currentType == ZoneType.sphere)
                colliders = Physics.OverlapSphere(zonePosition, zoneRadius, playerLayer, QueryTriggerInteraction.Ignore);


            if (colliders.Length == 1 && gameManager.player0 != null && gameManager.player1 != null)
            {
                if (blocksPlayers)
                {
                    if (colliders[0].gameObject.GetComponent<PlayerController>().playerIndex == 0)
                        gameManager.player1.transform.position = gameManager.player0.playerSpawnPoint.position;

                    else if (colliders[0].gameObject.GetComponent<PlayerController>().playerIndex == 1)
                        gameManager.player0.transform.position = gameManager.player1.playerSpawnPoint.position;
                }
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

            if (timeStamp > 0)
                timeStamp -= Time.deltaTime;

            else
            {
                wallL.SetActive(false);
                wallR.SetActive(false);
            }
        }
    }

    private void NonBlockingCam()
    {
        if (playerInsideTrigger && !blocksPlayers)
        {
            blockTriggered = true;

            if (currentType == ZoneType.box)
                collidersCam = Physics.OverlapBox(zonePosition, zoneSize / 2, gameObject.transform.rotation, playerLayer, QueryTriggerInteraction.Ignore);

            if (currentType == ZoneType.sphere)
                collidersCam = Physics.OverlapSphere(zonePosition, zoneRadius, playerLayer, QueryTriggerInteraction.Ignore);

            if (collidersCam.Length == 2 && gameManager.player0 != null && gameManager.player1 != null)
                disableNonBlockingCam = false;

            else if (collidersCam.Length == 1 && gameManager.player0 != null && gameManager.player1 != null)
                disableNonBlockingCam = true;

            else if (collidersCam.Length == 1 && gameManager.player0 != null && gameManager.player1 == null)
                disableNonBlockingCam = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (currentType == ZoneType.box)
        {
            Gizmos.DrawWireCube(zonePositionOffset, zoneSize);
            Gizmos.DrawWireCube(zonePositionOffset, triggerSize);
        }

        if (currentType == ZoneType.sphere)
        {
            Gizmos.DrawWireSphere(zonePositionOffset, zoneRadius);
            Gizmos.DrawWireSphere(zonePositionOffset, triggerRadius);
        }

        if (controlsCam)
            Gizmos.DrawCube(cameraPositionOffset,Vector3.one);

        if (automaticWall)
        {
            if(currentType == ZoneType.box)
            {
                Gizmos.DrawCube(new Vector3 (zonePositionOffset.x - zoneSize.x/ 2, 2, 0), new Vector3(1, 10, zoneSize.z));
                Gizmos.DrawCube(new Vector3(zonePositionOffset.x + zoneSize.x/ 2, 2, 0), new Vector3(1, 10, zoneSize.z));
            }

            if (currentType == ZoneType.sphere)
            {
                Gizmos.DrawCube(new Vector3(zonePositionOffset.x - zoneRadius, 2, 0), new Vector3(1, 10, zoneRadius * 2));
                Gizmos.DrawCube(new Vector3(zonePositionOffset.x + zoneRadius, 2, 0), new Vector3(1, 10, zoneRadius * 2));
            }
        }
    }
}
