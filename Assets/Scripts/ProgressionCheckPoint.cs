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
    [SerializeField] Material activeMat;
    Vector3 accelerationForce = new Vector3(-10,0,0);
    Vector3 randomForce = new Vector3(-15,0,0);
    GameManager gameManager;
    Cloth cloth;
    SkinnedMeshRenderer renderer;
    ParticleSystem p_confetis;
    Animator anim;
    AudioSource audioSource;
    int playerLayer;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerLayer = LayerMask.GetMask("Player");
        cloth = gameObject.GetComponentInChildren<Cloth>();
        renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        p_confetis = gameObject.GetComponentInChildren<ParticleSystem>();
        anim = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    //Unity Cycles : Check if the player is in the checkpoint and assign the respawn position to the GameManager
    private void FixedUpdate()
    {
        playerInside = Physics.CheckBox(transform.position + triggerZoneOffset, triggerSize/2,transform.rotation, playerLayer);

        if (playerInside && !triggered)
        {
            triggered = true;

            Vector3 respawnPosition;

            respawnPosition = transform.position + respawnPositionOffset;
            gameManager.currentProgressionCp = respawnPosition;
            gameManager.GetThroughSpawner(scoreAmount);

            renderer.material = activeMat;
            cloth.externalAcceleration = accelerationForce;
            cloth.randomAcceleration = randomForce;
            p_confetis.Play();
            anim.SetTrigger("Take");
            audioSource.Play();

            this.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(triggerZoneOffset, triggerSize);
        Gizmos.DrawSphere(respawnPositionOffset, 0.3f);
    }
}
