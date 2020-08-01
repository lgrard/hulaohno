using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    bool triggered = false;
    bool playerInside;
    [SerializeField] Vector3 triggerZoneOffset;
    [SerializeField] Vector3 triggerSize;
    GameManager gameManager;
    int playerLayer;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerLayer = LayerMask.GetMask("Player");
    }

    //Unity Cycles : Check if the player is in the checkpoint and end the level
    private void FixedUpdate()
    {
        playerInside = Physics.CheckBox(transform.position + triggerZoneOffset, triggerSize / 2, Quaternion.identity, playerLayer);

        if (playerInside && !triggered)
        {
            triggered = true;
            gameManager.LevelEnd();
            this.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + triggerZoneOffset, triggerSize);
    }
}

