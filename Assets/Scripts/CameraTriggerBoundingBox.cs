using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Policy;
using UnityEngine.SocialPlatforms;
using System.Runtime.Remoting.Channels;

public class CameraTriggerBoundingBox : MonoBehaviour
{
    private Vector3 center;
    public float radius = 10f;
    public Collider[] hitColliders;
    //public string collisionLayer1 = "Vehicles";
    //public string collisionLayer2 = "Pedestrian";
    public LayerMask layerMask;
    public LayerMask raycastLayerMask;
    public GameObject cameraObject;
    public Camera cameraComponent;
    private int stateTrack = 0;
    public int colliderNum = 0;
    public float margin = 0;
    private Vector3[] pts = new Vector3[8];
    public List<Collider> frontalHitColliders;
    public float cameraHorizontalFOV;
    // Start is called before the first frame update
    void Start()
    {
        //layerMask = (1 << LayerMask.NameToLayer(collisionLayer1)) | (1 << LayerMask.NameToLayer(collisionLayer2));

        cameraObject = this.gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();






    }

    // Update is called once per frame
    void Update()
    {

        var radAngle = cameraComponent.fieldOfView * Mathf.Deg2Rad;
        var radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * cameraComponent.aspect);
        var radVFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2));
        cameraHorizontalFOV = Mathf.Rad2Deg * radHFOV;

        frontalHitColliders = new List<Collider>();

        center = transform.position;

        if (Time.frameCount > 5)
        {


            hitColliders = Physics.OverlapSphere(center, radius, layerMask);

            /*

            foreach (Collider collider in hitColliders)
            {

                Vector3 localSpace = transform.InverseTransformPoint(collider.gameObject.transform.position);


                if (localSpace.z > 5)
                {
                    float theta = (Mathf.Atan(localSpace.z / Mathf.Abs(localSpace.x))) * Mathf.Rad2Deg;
                    Debug.Log(theta);

                    if (theta <= 90 && theta >= 90 - 0.8 * cameraHorizontalFOV)
                    {
                        frontalHitColliders.Add(collider);
                    }
                }

            }

            */
            foreach (Collider collider in hitColliders)
            {

                //Vector3 screenPos = cameraComponent.WorldToScreenPoint(collider.transform.position);

                Vector3 screenPoint = GetComponent<Camera>().WorldToViewportPoint(collider.gameObject.transform.position);

                bool onScreen = screenPoint.z > 0.75f && screenPoint.x > 0.2f && screenPoint.x < 0.8f && screenPoint.y > 0.2f && screenPoint.y < 0.8f;

                //bool onScreen = screenPos.x < cameraComponent.pixelWidth && screenPos.y < cameraComponent.pixelHeight && screenPos.z > 1f;

                RaycastHit hit;
                bool validRayCast = false;

                if (Physics.Raycast(transform.position, (collider.transform.position - transform.position), out hit, Mathf.Infinity, raycastLayerMask))
                {
                    if (hit.collider == collider)
                    {
                        validRayCast = true;
                    }
                }

                /*

                if (onScreen == true && validRayCast)
                {
                    frontalHitColliders.Add(collider);
                }
                */

                if (onScreen == true)
                {
                    frontalHitColliders.Add(collider);
                }


            }


            /*

            colliderNum = frontalHitColliders.Count;

            if (colliderNum == 0)
            {
                stateTrack++;
                cameraObject.GetComponent<TimeStamp>().cameraState = "OFF";
                cameraComponent.enabled = false;
            }

            else
            {
                cameraObject.GetComponent<TimeStamp>().cameraState = "ON";
                cameraComponent.enabled = true;
            }

            */

        }




        

    }
    void OnDrawGizmos()
    {
        cameraObject = this.gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();

        // Draw a yellow sphere at the transform's position

        Color sphereColor;




        if (cameraComponent.enabled == false)
        {
            sphereColor = Color.red;
            sphereColor.a = 0.1f;

        }

        else
        {
            sphereColor = Color.green;
            sphereColor.a = 0.1f;
        }

        Gizmos.color = sphereColor;

        Gizmos.DrawSphere(transform.position, radius);

        Handles.Label(cameraObject.transform.position, cameraObject.name);


    }


}
