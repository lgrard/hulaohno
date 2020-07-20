using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : MonoBehaviour
{
    [SerializeField] Mesh[] meshes;
    private MeshFilter meshFilter;
    private GameManager gameManager;
    [SerializeField] int scoreAmount;
    [SerializeField] Material moboMat;

    private void Start()
    {
        meshFilter = gameObject.GetComponentInChildren<MeshFilter>();

        int randomMesh = Random.Range(0, meshes.Length);
        meshFilter.mesh = meshes[randomMesh];

        if(randomMesh == 4)
            gameObject.GetComponentInChildren<MeshRenderer>().material = moboMat;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameObject.transform.localScale = new Vector3(1, 1, 1) * Random.Range(0.75f, 1.25f);

        gameObject.GetComponent<Animator>().SetFloat("CycleOffset",Random.Range(0f,1f));
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameManager.Scoring(scoreAmount);
            gameObject.GetComponent<Animator>().SetTrigger("Take");
            Destroy(gameObject,0.2f);
        }
    }
}
