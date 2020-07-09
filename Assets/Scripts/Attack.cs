using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] string attackName;
    public int attackIndex;
    public Vector3 attackOffset;
    public float attackRadius;
    public int attackDamage;
}
