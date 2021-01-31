using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using TMPro;

public class NPCManagement : MonoBehaviour
{
    [SerializeField] GameObject dialogueBoxContainer;
    [SerializeField] Animator meshAnim;

    [Header("Values")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float zoneRadius = 5;
    [SerializeField] Type currentType;
    private enum Type
    {
        displayText,
        Sign,
        displayTextAndSign,
        doesNothing,
    }
    
    private Animator anim;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        dialogueBoxContainer.SetActive(false);
    }

    private void FixedUpdate()
    {
        bool playerInside = Physics.CheckSphere(transform.position, zoneRadius, playerLayer, QueryTriggerInteraction.Ignore);

        if(currentType == Type.displayText || currentType == Type.displayTextAndSign)
            anim.SetBool("Opened", playerInside);

        if (currentType == Type.displayTextAndSign || currentType == Type.Sign)
            meshAnim.SetBool("Sign", playerInside);
    }

    private void LateUpdate()
    {
        if(currentType == Type.displayText || currentType == Type.displayTextAndSign)
            dialogueBoxContainer.transform.LookAt(Camera.main.transform);
    }

    private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(transform.position, zoneRadius);
}
