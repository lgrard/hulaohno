using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDoor : MonoBehaviour
{
    public bool isOpenning;
    [SerializeField] GameObject door;
    [SerializeField] Transform waypointsA;
    [SerializeField] Transform waypointsB;
    [SerializeField] float openningSpeed = 1f;
    [SerializeField] float closingSpeed = 1f;
    private float progress = 0;

    private void Start()
    {
        door.transform.position = waypointsA.position;
    }

    private void FixedUpdate()
    {
        door.transform.position = Vector3.Lerp(waypointsA.position, waypointsB.position, progress);

        if (isOpenning && progress < 1)
        {
            progress += Time.deltaTime * openningSpeed;
        }

        else if(!isOpenning && progress > 0)
        {
            progress -= Time.deltaTime * closingSpeed;
        }
    }
}
