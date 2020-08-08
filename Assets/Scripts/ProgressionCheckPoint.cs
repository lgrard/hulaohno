using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionCheckPoint : MonoBehaviour
{
    bool triggered = false;
    bool playerInside;
    [SerializeField] int scoreAmount = 50;
    [SerializeField] Vector3 respawnPositionOffset;
    [SerializeField] Vector3 triggerZoneOffset;
    [SerializeField] Vector3 triggerSize;
    GameManager gameManager;
    int playerLayer;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerLayer = LayerMask.GetMask("Player");
    }

    //Unity Cycles : Check if the player is in the checkpoint and assign the respawn position to the GameManager
    private void FixedUpdate()
    {
        playerInside = Physics.CheckBox(transform.position + triggerZoneOffset, triggerSize/2,Quaternion.identity, playerLayer);

        if (playerInside && !triggered)
        {
            triggered = true;

            Vector3 respawnPosition;

            respawnPosition = transform.position + respawnPositionOffset;
            gameManager.currentProgressionCp = respawnPosition;
            gameManager.GetThroughSpawner(scoreAmount);
            this.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + triggerZoneOffset, triggerSize);
        Gizmos.DrawSphere(transform.position + respawnPositionOffset, 0.3f);
    }
}
