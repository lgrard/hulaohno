using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float delay = 2f;
    private GameObject platform;
    [SerializeField] Transform waypointsA;
    [SerializeField] Transform waypointsB;
    private bool aToB = true;
    private float progress;

    private void Start()
    {
        platform = gameObject;
        platform.transform.localPosition = waypointsA.localPosition;
    }

    private void FixedUpdate()
    {
        platform.transform.localPosition = Vector3.Lerp(waypointsA.localPosition, waypointsB.localPosition, progress);

        StartCoroutine(Move());
    }

    //add delay to the platform movement
    private IEnumerator Move()
    {
        if (aToB && progress <= 1)
            progress += Time.deltaTime * speed;

        else if(!aToB && progress >= 0)
            progress -= Time.deltaTime * speed;


        if (progress <= 0)
        {
            yield return new WaitForSeconds(delay);
            aToB = true;
        }

        else if (progress >= 1)
        {
            yield return new WaitForSeconds(delay);
            aToB = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawSphere(waypointsA.position, 0.2f);
        Gizmos.DrawSphere(waypointsB.position, 0.2f);
        Gizmos.DrawLine(waypointsA.position, waypointsB.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerController) && collision.collider.GetType() == typeof(CapsuleCollider))
            collision.gameObject.transform.SetParent(platform.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController playerController) && collision.collider.GetType() == typeof(CapsuleCollider))
            collision.gameObject.transform.SetParent(null);
    }
}
