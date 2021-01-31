using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : MonoBehaviour
{
    [SerializeField] Mesh[] meshes = null;
    
    MeshFilter meshFilter = null;
    GameManager gameManager = null;
    AudioSource audioSource = null;

    bool isTaken = false;
    float progress = 0;
    Transform target = null;
    
    [SerializeField] int scoreAmount = 0;
    [SerializeField] Material moboMat = null;
    [SerializeField] ParticleSystem p_take = null;

    private void Start()
    {
        meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
        audioSource = gameObject.GetComponent<AudioSource>();

        int randomMesh = Random.Range(0, meshes.Length);
        meshFilter.mesh = meshes[randomMesh];

        if(randomMesh == 4)
            gameObject.GetComponentInChildren<MeshRenderer>().material = moboMat;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameObject.transform.localScale = new Vector3(1, 1, 1) * Random.Range(0.75f, 1.25f);

        gameObject.GetComponent<Animator>().SetFloat("CycleOffset",Random.Range(0f,1f));
    }

    private void Update()
    {
        if (isTaken && progress < 1 && target != null)
        {
            progress += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.transform.position, progress);
        }

        else if (progress >= 1)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController) && collision.GetType() == typeof(CapsuleCollider))
        {
            gameObject.GetComponent<Animator>().SetTrigger("Take");
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
            target = collision.transform;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            p_take.Play();

            if (gameManager.currentEvent != null && gameManager.currentEvent.currentType == Events.EventsType.pickUpItems)
                gameManager.currentEvent.amountLeft += 1;

            if(playerController.playerIndex == 0)
                gameManager.Scoring1(scoreAmount, false);
            else
                gameManager.Scoring2(scoreAmount, false);

            isTaken = true;
        }
    }
}
