using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraEffects : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject cameraContainer;
    [SerializeField] float smoothingAmount = 0.95f;
    [SerializeField] GameObject cameraTarget;

    [Header("Distance management")]
    [SerializeField] BoxCollider wallL = null;
    [SerializeField] BoxCollider wallR = null;
    [SerializeField] LineRenderer distanceLine;
    [SerializeField] float lineOffset = 0.5f;
    [SerializeField] float minDistanceCam = 16;
    [SerializeField] float maxDistanceCam = 19;

    private GameManager gameManager;
    [SerializeField] Vector3 offset;
    private Camera cam;
    private Vector3 rotOffset;

    public bool checkPointActive = false;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.player0 != null)
            cameraTarget.transform.position = gameManager.player0.transform.position;

        cam = Camera.main;
        
        //Offset auto setting at start
        //offset  = cameraContainer.transform.position - cameraTarget.transform.position;
        
        offset = new Vector3(0, 6.5f, -minDistanceCam);        
        rotOffset = cameraContainer.transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        WallSetting();

        if (cameraTarget != null && !checkPointActive && gameManager.player0 != null)
        {

            if (gameManager.player0 != null && !gameManager.p1IsDead && gameManager.player1 != null && !gameManager.p2IsDead)
            {
                cameraTarget.transform.position = Vector3.Lerp(gameManager.player0.transform.position, gameManager.player1.transform.position, 0.5f);
                DistanceSetting();
            }

            else if (gameManager.player0 == null && gameManager.player1 != null || gameManager.player0 != null && gameManager.p1IsDead && gameManager.player1 != null && !gameManager.p2IsDead)
                cameraTarget.transform.position = gameManager.player1.transform.position;

            else if (gameManager.player1 == null && gameManager.player0 != null || gameManager.player1 != null && gameManager.p2IsDead && gameManager.player0 != null && !gameManager.p1IsDead)
                cameraTarget.transform.position = gameManager.player0.transform.position;


            //Vector3 desiredPosition = new Vector3(cameraTarget.transform.position.x + offset.x, cameraTarget.transform.position.y + offset.y, offset.z);
            Vector3 desiredPosition = new Vector3(cameraTarget.transform.position.x + offset.x, cameraTarget.transform.position.y + offset.y, cameraTarget.transform.position.z + offset.z);
            cameraContainer.transform.position = Vector3.Lerp(desiredPosition, cameraContainer.transform.position, smoothingAmount);

            Vector3 desiredRotation = rotOffset;
            cameraContainer.transform.rotation = Quaternion.Slerp(Quaternion.Euler(desiredRotation), cameraContainer.transform.rotation,smoothingAmount);
        }
    }

    void WallSetting()
    {
        wallL.enabled = gameManager.playerOutRange;
        wallR.enabled = gameManager.playerOutRange;

        distanceLine.enabled = gameManager.playerOutRange;

        if (gameManager.playerOutRange)
        {
            distanceLine.widthMultiplier = gameManager.distanceRatio-1;

            Vector3 player0Pos = new Vector3(gameManager.player0.transform.position.x, gameManager.player0.transform.position.y + lineOffset, gameManager.player0.transform.position.z);
            Vector3 player1Pos = new Vector3(gameManager.player1.transform.position.x, gameManager.player1.transform.position.y + lineOffset, gameManager.player1.transform.position.z);

            distanceLine.SetPosition(0, player0Pos);
            distanceLine.SetPosition(1, Vector3.Lerp(player0Pos, player1Pos, 0.5f));
            distanceLine.SetPosition(2, player1Pos);
        }
    }

    void DistanceSetting()
{
        float distanceCam = Mathf.Clamp(gameManager.distanceRatio * maxDistanceCam, minDistanceCam, maxDistanceCam);
        offset = new Vector3(offset.x,offset.y, -distanceCam);
    }

    // Shake Function
    public IEnumerator Shake (float Duration, float Force)
    {
        cam = Camera.main;

        Vector3 InitialPosition = cam.transform.localPosition;
        while (Duration > 0)
        {
            float x = Random.Range(-1f, 1f) * Force;
            float y = Random.Range(-1f, 1f) * Force;
            cam.transform.localPosition = new Vector3(x, y, InitialPosition.z);
            Duration -= Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = InitialPosition;
    }

    //HitStop Function
    public IEnumerator Hitstop (float Duration)
    {
        cam = Camera.main;

        while (Duration > 0)
        {
            Time.timeScale = 0.02f;
            Duration -= Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1;
    }

    //Zoom Function
    public IEnumerator Zoom (float Amount, float Speed)
    {
        cam = Camera.main;

        float initialFov = cam.orthographicSize;
        cam.orthographicSize = cam.orthographicSize - Amount;

        while (cam.orthographicSize < initialFov)
        {
            cam.orthographicSize += Time.unscaledDeltaTime * Speed;
            yield return null;
        }

        cam.orthographicSize = initialFov;
    }
}
