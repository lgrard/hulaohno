using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] int damage = 1000;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerController playerController) && collider.GetType() == typeof(CapsuleCollider))
        {
            playerController.TakeDamage(damage);
        }
    }
}
