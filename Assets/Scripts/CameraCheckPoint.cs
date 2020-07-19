using System.Collections;
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

    bool blockTriggered = false;
    bool playerInsideTrigger;
    bool playerInside;
    float timeStamp = 1;
    int playerLayer;
    Camera cam;



    //Initialize
    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        cam = Camera.main;

        foreach(EnemySpawner spawner in linkedSpawner)
        {
            spawner.isSpawning = true;
            spawner.enabled = false;
        }
    }

    //Update method
    private void Update()
    {
        triggerSize = new Vector3(zoneSize.x - triggerOffset, zoneSize.y, zoneSize.z - triggerOffset);

        wallL.transform.localScale = new Vector3(1,10,zoneSize.z);
        wallL.transform.localPosition = new Vector3(zoneSize.x / 2 + wallL.transform.localScale.x/2, 2, 0);

        wallR.transform.localScale = new Vector3(2, 10, zoneSize.z);
        wallR.transform.localPosition = new Vector3(-zoneSize.x / 2 - wallL.transform.localScale.x / 2, 2, 0);

        gameObject.transform.position = zonePosition;
    }

    //Fixed Update method
    private void FixedUpdate()
    {
        Blocking();

        playerInsideTrigger = Physics.CheckBox(zonePosition, triggerSize/ 2,Quaternion.identity,playerLayer);
        playerInside = Physics.CheckBox(zonePosition, zoneSize / 2, Quaternion.identity, playerLayer);

        if (blockTriggered && playerInside && controlsCam)
        {
            cam.GetComponentInParent<CameraEffects>().checkPointActive = true;
            Transform camContainer = cam.GetComponentInParent<CameraEffects>().transform;
            camContainer.position = Vector3.Lerp(cam.transform.position, cameraPosition, 0.1f);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(cameraRotation), 0.1f);
        }

        else
            cam.GetComponentInParent<CameraEffects>().checkPointActive = false;
    }

    //Blocking method
    private void Blocking()
    {
        if (playerInsideTrigger && !blockTriggered)
        {
            blockTriggered = true;

            if (blocksPlayers)
            {
                wallL.SetActive(true);
                wallR.SetActive(true);
            }

            foreach (EnemySpawner spawner in linkedSpawner)
                spawner.enabled = true;
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
