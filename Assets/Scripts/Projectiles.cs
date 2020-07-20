using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] int damage = 2;
    [SerializeField] float lifeTime = 8f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.GetComponent<PlayerController>().TakeDamage(damage);
            
            Destroy(gameObject);
        }

        else if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject, 0.5f);
        }
    }

}
