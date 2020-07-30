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
    private GameManager gameManager;
    private Vector3 offset;
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
        
        offset = new Vector3(0, 6.5f, cameraContainer.transform.position.z);        
        rotOffset = cam.transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        if (gameManager.player0 != null && gameManager.player1 != null)
            cameraTarget.transform.position = Vector3.Lerp(gameManager.player0.transform.position, gameManager.player1.transform.position, 0.5f);

        else if (gameManager.player0 == null && gameManager.player1 != null)
            cameraTarget.transform.position = gameManager.player1.transform.position;

        else if (gameManager.player1 == null && gameManager.player0 != null)
            cameraTarget.transform.position = gameManager.player0.transform.position;


        if (cameraTarget != null && !checkPointActive && gameManager.player0 != null)
        {
            Vector3 desiredPosition = new Vector3(cameraTarget.transform.position.x + offset.x, cameraTarget.transform.position.y + offset.y, offset.z);
            cameraContainer.transform.position = Vector3.Lerp(desiredPosition, cameraContainer.transform.position, smoothingAmount);

            Vector3 desiredRotation = rotOffset;
            cam.transform.rotation = Quaternion.Slerp(Quaternion.Euler(desiredRotation), cam.transform.rotation,smoothingAmount);
        }
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
