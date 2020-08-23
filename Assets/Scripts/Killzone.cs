using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Killzone : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] int damage = 1000;
    [SerializeField] float fireRate = 50;
    [SerializeField] Vector3 size;

    [Header("Visuals")]
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] ParticleSystem p_fire;
    [SerializeField] ParticleSystem p_fire1;
    [SerializeField] GameObject liquidPlane;

    [Header("Type")]
    [SerializeField] KillzoneType currentType;
    enum KillzoneType
    {
        fire,
        liquid,
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {

            boxCollider.size = size;
            boxCollider.center = new Vector3(0, size.y / 2, 0);

            if(currentType == KillzoneType.fire)
            {
                var shape = p_fire.shape;
                shape.scale = new Vector3(size.x,size.z,size.y);
                p_fire.emissionRate = fireRate * size.x * size.z;

                var shape1 = p_fire1.shape;
                shape1.scale = new Vector3(size.x, size.z, size.y);
                p_fire1.emissionRate = fireRate/10 * size.x * size.z;

                transform.localScale = Vector3.one;
            }

            else
            {
                liquidPlane.transform.localScale = new Vector3 (size.x/10,0.5f,size.z/10);
                liquidPlane.transform.localPosition = new Vector3(0, size.y, 0);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerController playerController) && collider.GetType() == typeof(CapsuleCollider))
        {
            playerController.TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 offset = new Vector3(0, size.y/2, 0);
        Gizmos.DrawCube(Vector3.zero + offset, size);
    }
}
