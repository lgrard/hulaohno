using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingSwitch : MonoBehaviour
{
    [SerializeField] Vector3 triggerSize;
    [SerializeField] Vector3 triggerOffset;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject switchMesh;
    AudioSource audioSource;
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

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        MeshBevahiour();

        if (playerInside && !triggered && linkedObjects != null)
        {
            triggered = true;
            audioSource.Play();

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
        {
            audioSource.Play();
            triggered = false;
        }
    }

    void MeshBevahiour()
    {
        if(playerInside && triggered)
            switchMesh.transform.localPosition = new Vector2(0, 0);

        if (!playerInside && !triggered)
            switchMesh.transform.localPosition = new Vector2(0, 0.001f);
    }

    IEnumerator OpenDoor()
    {
        while (playerInside)
        {
            foreach(GameObject linkedObject in linkedObjects)
            {
                linkedObject.GetComponent<SwitchDoor>().isOpenning = true;
                yield return new WaitForEndOfFrame();
            }
        }

        foreach(GameObject linkedObject in linkedObjects)
        {
            linkedObject.GetComponent<SwitchDoor>().isOpenning = false;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator movePlatform()
    {
        while (playerInside)
        {
            foreach (GameObject linkedObject in linkedObjects)
            {
                linkedObject.GetComponentInChildren<MovingPlatform>().isMoving = true;
                yield return new WaitForEndOfFrame();
            }
        }

        foreach (GameObject linkedObject in linkedObjects)
        {
            linkedObject.GetComponentInChildren<MovingPlatform>().isMoving = false;
            yield return new WaitForEndOfFrame();
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
