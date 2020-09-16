using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateActivate : MonoBehaviour
{

    public GameObject cameraObject;
    public Camera cameraComponent;

    // Start is called before the first frame update
    void Start()
    {
        cameraObject = this.gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();
        cameraComponent.enabled = true;


        
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.frameCount == 5)
        {

            cameraComponent.enabled = false;

        }






    }
}
