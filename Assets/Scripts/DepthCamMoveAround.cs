using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCamMoveAround : MonoBehaviour
{

    public bool enable = false;
    public float speed = 0f;
    public GameObject rootWaypoint;
    private int waypointCount;
    private int index;
    private Transform targetWaypoint;
    // Start is called before the first frame update
    void Start()
    {
        waypointCount = rootWaypoint.transform.childCount;

        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enable == true)
        {
            targetWaypoint = rootWaypoint.transform.GetChild(index);

            Debug.Log(targetWaypoint);

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetWaypoint.position - transform.position), Time.deltaTime);

            if (transform.position == targetWaypoint.position)
            {
                if (index == waypointCount - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
            }
        }

    }
}
