using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Waypoint : MonoBehaviour
{
    public float debugRadius = 0.5f;


    public void Start()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {


            float difference = hit.distance;

            transform.position = transform.position + Vector3.down * difference;

        }

        else
        {
            Debug.Log("none");
        }
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, debugRadius);
        Handles.Label(transform.position, transform.gameObject.name);

    }

}
