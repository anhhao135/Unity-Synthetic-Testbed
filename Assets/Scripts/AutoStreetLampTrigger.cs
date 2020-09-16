using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class AutoStreetLampTrigger : MonoBehaviour
{

    public GameObject lightingController;
    public float onStartTime = 15;
    public float onStopTime = 8;
    float timeOfDay;
    // Start is called before the first frame update
    void Start()
    {

        lightingController = GameObject.Find("DemoController");

    }

    // Update is called once per frame
    void Update()
    {
        //timeOfDay = lightingController.GetComponent<LightingController>().timeOfDay;

        if (timeOfDay > onStartTime || timeOfDay < onStopTime)
        {
            this.GetComponent<Light>().enabled = true;
        }

        else
        {
            this.GetComponent<Light>().enabled = false;
        }

    }
}
