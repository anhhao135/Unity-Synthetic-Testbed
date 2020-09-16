using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DemoController : MonoBehaviour
{

    public bool pointCloud = false;
    public bool camParam = false;
    public bool opticalFlow = false;
    public bool FPSControl = false;
    public bool car_RGB = false;

    public GameObject depthCamera;
    public GameObject pointCloudVisual;
    public GameObject pointCloudCamera;

    public GameObject apertureCam;
    public PostProcessProfile profile;
    [Range(0.1f, 20f)] public float aperture = 2.8f;
    [Range(0.1f, 30f)] public float focusDistance = 3f;
    public float FOV = 60f;


    public GameObject opticalFlowCam;

    public GameObject firstPersonController;

    public GameObject carRGB1;
    public GameObject carRGB2;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        profile.GetSetting<DepthOfField>().aperture.value = 20f;
        profile.GetSetting<DepthOfField>().focusDistance.value = 10f;

        if (pointCloud == true)
        {
            depthCamera.SetActive(true);
            pointCloudVisual.SetActive(true);
            pointCloudCamera.SetActive(true);

        }

        else
        {
            depthCamera.SetActive(false);
            pointCloudVisual.SetActive(false);
            pointCloudCamera.SetActive(false);
        }


        if (camParam == true)
        {
            apertureCam.SetActive(true);
            profile.GetSetting<DepthOfField>().aperture.value = aperture;
            profile.GetSetting<DepthOfField>().focusDistance.value = focusDistance;
            apertureCam.GetComponent<Camera>().fieldOfView = FOV;

        }

        else
        {
            apertureCam.SetActive(false);
        }

        if (opticalFlow == true)
        {

            opticalFlowCam.SetActive(true);

        }

        else
        {
            opticalFlowCam.SetActive(false);
        }

        if (FPSControl == true)
        {

            firstPersonController.SetActive(true);

        }

        else
        {
            firstPersonController.SetActive(false);
        }

        if (car_RGB == true)
        {

            carRGB1.SetActive(true);
            carRGB2.SetActive(true);

        }

        else
        {
            carRGB1.SetActive(false);
            carRGB2.SetActive(false     );
        }
    }
}
