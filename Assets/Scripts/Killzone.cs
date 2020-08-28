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

    [Header("Ripple")]
    [SerializeField] Transform wayPointA;
    [SerializeField] Transform wayPointB;
    [SerializeField] GameObject rippleMesh;
    [SerializeField] float rippleSpeed = 0.2f;
    [SerializeField] float rippleDelay = 0f;
    [SerializeField] bool previewRipple;
    private Vector3 rippleSize;
    private float progress;

    [Header("Type")]
    [SerializeField] KillzoneType currentType;
    enum KillzoneType
    {
        fire,
        liquid,
        ripple,
    }

    private void Start()
    {
        progress += rippleDelay;
        rippleSize = size;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {

            if(currentType == KillzoneType.fire)
            {
                boxCollider.size = size;
                boxCollider.center = new Vector3(0, size.y / 2, 0);
                var shape = p_fire.shape;
                shape.scale = new Vector3(size.x,size.z,size.y);
                p_fire.emissionRate = fireRate * size.x * size.z;

                var shape1 = p_fire1.shape;
                shape1.scale = new Vector3(size.x, size.z, size.y);
                p_fire1.emissionRate = fireRate/10 * size.x * size.z;

                transform.localScale = Vector3.one;
            }

            else if (currentType == KillzoneType.liquid)
            {
                boxCollider.size = size;
                boxCollider.center = new Vector3(0, size.y / 2, 0);
                liquidPlane.transform.localScale = new Vector3 (size.x/10,0.5f,size.z/10);
                liquidPlane.transform.localPosition = new Vector3(0, size.y, 0);
            }

            else if(currentType == KillzoneType.ripple && wayPointA != null && wayPointB != null)
            {
                boxCollider.size = size;
                boxCollider.center = new Vector3(rippleMesh.transform.localPosition.x, rippleMesh.transform.localPosition.y + size.y / 2, rippleMesh.transform.localPosition.z);

                if (previewRipple)
                    rippleMesh.transform.position = Vector3.Lerp(wayPointA.position, wayPointB.position, progress);

                else
                    rippleMesh.transform.position = wayPointA.transform.position;

                rippleMesh.transform.localScale = size/5;

                if (progress < 1)
                    progress += Time.fixedDeltaTime* rippleSpeed;

                else
                    progress = 0;
            }
        }

        if (currentType == KillzoneType.ripple && wayPointA != null && wayPointB != null && Application.isPlaying)
        {
            rippleMesh.transform.localScale = size / 5;
            boxCollider.center = new Vector3(rippleMesh.transform.localPosition.x, rippleMesh.transform.localPosition.y + size.y / 2, rippleMesh.transform.localPosition.z);
            rippleMesh.transform.position = Vector3.Lerp(wayPointA.position, wayPointB.position, progress);

            if (progress < 1)
            {
                progress += Time.fixedDeltaTime* rippleSpeed;

                if(size.x < rippleSize.x)
                    size = new Vector3(size.x + Time.fixedDeltaTime * 10,size.y,size.z);
                if (size.y < rippleSize.y)
                    size = new Vector3(size.x, size.y + Time.fixedDeltaTime * 10, size.z);
                if (size.z < rippleSize.z)
                    size = new Vector3(size.x, size.y, size.z + Time.fixedDeltaTime * 10);
            }

            else
            {
                size = Vector3.zero;
                progress = 0;
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

        if(currentType == KillzoneType.fire || currentType == KillzoneType.liquid)
        {
            Vector3 offset = new Vector3(0, size.y/2, 0);
            Gizmos.DrawCube(Vector3.zero + offset, size);
        }

        else if(currentType == KillzoneType.ripple && wayPointA != null && wayPointB != null)
        {
            Gizmos.DrawCube(wayPointA.transform.localPosition,Vector3.one*0.1f);
            Gizmos.DrawCube(wayPointB.transform.localPosition, Vector3.one * 0.1f);
            Gizmos.DrawLine(wayPointA.transform.localPosition, wayPointB.transform.localPosition);

            Gizmos.DrawCube(new Vector3(rippleMesh.transform.localPosition.x, rippleMesh.transform.localPosition.y + size.y / 2, rippleMesh.transform.localPosition.z), size);
        }
    }
}
