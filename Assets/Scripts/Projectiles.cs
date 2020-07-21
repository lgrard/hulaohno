using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float lifeTime = 8f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            print("projectiles hit ground");
            Destroy(gameObject, 0.3f);
        }
    }

}
