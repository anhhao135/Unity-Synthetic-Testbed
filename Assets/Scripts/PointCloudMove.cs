using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudMove : MonoBehaviour
{

    public Transform depthCamera;
    Vector3 relative;
    // Start is called before the first frame update
    void Start()
    {
        relative = transform.position - depthCamera.position;

        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = depthCamera.position + relative;
        transform.rotation = depthCamera.rotation;
        
    }
}
