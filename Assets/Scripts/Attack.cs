using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] string attackName;
    public int attackIndex;
    public float attackRadius;
    public int attackDamage;
    public float delay;
    public Transform attackPosition;
    [SerializeField] bool displayPreview;

    private void OnDrawGizmosSelected()
    {
        if(displayPreview)
            Gizmos.DrawWireSphere(attackPosition.position, attackRadius);
    }
}
