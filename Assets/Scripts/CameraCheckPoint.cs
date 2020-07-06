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


    [Header("Does this zone uses blocks the players")]
    public bool blocksPlayers;

    [Header("Does this zone uses blocks the players")]
    public bool controlsCam;

    [Header("Linked Spawner (null if blocksPlayers = false)")]
    public List<EnemySpawner> linkedSpawner = new List<EnemySpawner>();
    private List<EnemySpawner> endedSpawner = new List<EnemySpawner>();

    [HideInInspector]
    public BoxCollider boundL;
    [HideInInspector]
    public BoxCollider boundR;

    bool blockTriggered = false;
    bool playerInside;
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
        boundL.size = new Vector3(2,10, zoneSize.z);
        boundL.center = new Vector3 (zoneSize.x/2+boundL.size.x/2.5f, 2,0);

        boundR.size = new Vector3(2, 10, zoneSize.z);
        boundR.center = new Vector3(-zoneSize.x/2-boundR.size.x/2.5f, 2, 0);

        gameObject.transform.position = zonePosition;
    }

    //Fixed Update method
    private void FixedUpdate()
    {
        Blocking();

        playerInside = Physics.CheckBox(zonePosition, zoneSize/2,Quaternion.identity,playerLayer);

        if (playerInside && controlsCam)
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
        if (playerInside && !blockTriggered)
        {
            blockTriggered = true;

            if (blocksPlayers)
            {
                boundL.enabled = true;
                boundR.enabled = true;
            }

            foreach (EnemySpawner spawner in linkedSpawner)
                spawner.enabled = true;
        }

        if (linkedSpawner.Capacity > 0 && linkedSpawner.Capacity > endedSpawner.Capacity)
        {
            foreach (EnemySpawner spawner in linkedSpawner)
            {
                if (spawner.enemyRemaining <= 0)
                    endedSpawner.Add(spawner);
            }
        }

        else if (blocksPlayers)
        {
            boundL.enabled = false;
            boundR.enabled = false;
        }
    }
 }
