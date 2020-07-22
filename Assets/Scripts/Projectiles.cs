using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float lifeTime = 8f;
    [SerializeField] float speed = 8f;
    [SerializeField] ParticleSystem p_hit;
    public Vector3 direction;
    Rigidbody rb;
    bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if(!hasHit)
            rb.velocity = direction * speed;
    }

    void HitSomething()
    {
        hasHit = true;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        rb.velocity = Vector3.zero;
        p_hit.Play();
        Destroy(gameObject,0.2f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            HitSomething();
            collider.GetComponent<PlayerController>().TakeDamage(damage);
        }

        else if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HitSomething();
        }
    }

}
