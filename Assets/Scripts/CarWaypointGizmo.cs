using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class CarWaypointGizmo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {

            RaycastHit hit;

            if (Physics.Raycast(child.position, child.TransformDirection(Vector3.down), out hit, Mathf.Infinity)) {


                float difference = hit.distance - 1.5f;

                child.position = child.position + Vector3.down * difference;

            }

            else
            {
                Debug.Log("none");
            }


        }

    }

    // Update is called once per frame
    void Update()
    {




    }

    public void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(child.position, child.position + Vector3.down * 1.5f);
            Gizmos.DrawSphere(child.position, 0.2f);
            Handles.Label(child.position + Vector3.up, child.gameObject.name);
            

        }
    }

}
