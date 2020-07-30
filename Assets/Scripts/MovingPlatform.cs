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
        if (progress <= 0.005 && progress >= -0.005 || progress >= 0.995 && progress <= 1.005)
            yield return new WaitForSeconds(delay);
        
            if (aToB)
                progress += Time.deltaTime * speed;

            else
                progress -= Time.deltaTime * speed;

            if (progress <= 0)
                aToB = true;

            else if (progress >= 1)
                aToB = false;
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
        if (collision.collider.CompareTag("Player"))
            collision.gameObject.transform.SetParent(platform.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.gameObject.transform.SetParent(null);
    }
}
