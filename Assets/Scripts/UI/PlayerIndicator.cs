using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    PlayerController playerController;
    GameManager gameManager;
    Camera cam;
    SpriteRenderer spriteRenderer;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float size = 0.6f;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        cam = Camera.main;
        ColorSet();
    }

    // Update is called once per frame
    void Update()
    {
        distance = size + Mathf.Clamp(Vector3.Distance(transform.position, cam.transform.position),1,20)/20;

        transform.LookAt(cam.transform);
        transform.localScale = new Vector3(distance,distance,distance);
    }

    private void FixedUpdate()
    {
        spriteRenderer.enabled = Physics.Linecast(playerController.transform.position, cam.transform.position,layerMask);
    }

    private void ColorSet()
    {
        if (playerController.playerIndex == 0)
            spriteRenderer.color = gameManager.player1Color;

        if (playerController.playerIndex == 1)
            spriteRenderer.color = gameManager.player2Color;
    }
}
