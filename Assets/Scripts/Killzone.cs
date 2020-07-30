using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] int damage = 1000;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetType() == typeof(CapsuleCollider))
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                collider.GetComponent<PlayerController>().TakeDamage(damage);
            }
        }
        
    }
}
