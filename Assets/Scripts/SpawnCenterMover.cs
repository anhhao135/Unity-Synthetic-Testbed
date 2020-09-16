using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnCenterMover : MonoBehaviour
{

    public List<GameObject> rootWaypoints;
    public int stayFrames;
    [SerializeField] int i;
    [SerializeField] int capturedFramesCount;
    int waypointIndex = 0;
    public UnityEvent spawnPedestrians;
    public UnityEvent destroyPedestrians;
    public UnityEvent spawnCars;
    public UnityEvent destroyCars;
    public UnityEvent captureFrame;
    public CarSpawner carSpawner;
    public PedestrianSpawner pedestrianSpawner;
    public List<GameObject> cameraViewCars = new List<GameObject>();
    bool cameraViewCarsRefresh = true;
    public Camera RGB_Bounding_Camera;
    int cameraViewCarsIndex = 0;
    [SerializeField] List<GameObject> moveWaypoints = new List<GameObject>();
    public bool toggleRandomCameraRotation = false;
    public bool toggleCycleDay = false;
    private int defaultHour = 12;
    
    // Start is called before the first frame update
    void Start()
    {

        UnityEngine.Random.InitState(351);

        foreach (GameObject rootWaypoint in rootWaypoints)
        {
            int i = 2;

            foreach (Transform child in rootWaypoint.transform)
            {

                if ( i%2 == 0)
                {
                    moveWaypoints.Add(child.gameObject);
                }

                i++;
                
            }
        }



        //Shuffle(moveWaypoints);

        transform.position = moveWaypoints[waypointIndex].transform.position;

        capturedFramesCount = 0;

        EnviroSkyMgr.instance.SetTimeOfDay(defaultHour);





    }

    void Shuffle(List<GameObject> list)
    {
        System.Random rng = new System.Random(135);

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public static float TriangleWave(float x)
    {
        return UnityEngine.Mathf.Abs((x % 4) - 2) - 1;
    }



    // Update is called once per frame
    void Update()
    {

        if (toggleCycleDay == true)
        {
            EnviroSkyMgr.instance.SetTimeOfDay(12f + 12f * TriangleWave(Time.frameCount / 50f));
        }

        if (Time.frameCount == 2)
        {
            spawnPedestrians.Invoke();
        }


        i++;

        if (cameraViewCarsRefresh == true)
        {
            foreach (GameObject car in carSpawner.spawnedCars)
            {
                if ((transform.position - car.transform.position).magnitude < pedestrianSpawner.centerSpawnDistance)
                {
                    cameraViewCars.Add(car);
                }
            }





            cameraViewCarsRefresh = false;
        }


        if (i > 2)
        {
            captureFrame.Invoke();
            capturedFramesCount++;
        }

        if (cameraViewCars.Count > 0)
        {
            RGB_Bounding_Camera.transform.position = cameraViewCars[cameraViewCarsIndex].transform.position + cameraViewCars[cameraViewCarsIndex].transform.up * 1.8f + cameraViewCars[cameraViewCarsIndex].transform.forward * 4f;
            RGB_Bounding_Camera.transform.rotation = cameraViewCars[cameraViewCarsIndex].transform.rotation;
            if (toggleRandomCameraRotation == true)
            {
                RGB_Bounding_Camera.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(-30f,30f), 0));
                RGB_Bounding_Camera.transform.Rotate(new Vector3(0, 0, UnityEngine.Random.Range(-10f,10f)));
            }
        }

        else
        {
            i = stayFrames;
        }
        








        if (cameraViewCarsIndex == cameraViewCars.Count - 1)
        {
            cameraViewCarsIndex = 0;
            destroyCars.Invoke();
            spawnCars.Invoke();

            cameraViewCars.Clear();

            cameraViewCarsRefresh = true;

            cameraViewCarsIndex = 0;
        }

        else
        {
            cameraViewCarsIndex++;

        }



        if (i == stayFrames)
        {

            if (waypointIndex == moveWaypoints.Count - 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }

            destroyPedestrians.Invoke();
            destroyCars.Invoke();

            waypointIndex++;

            transform.position = moveWaypoints[waypointIndex].transform.position;

            i = 0;

            spawnPedestrians.Invoke();
            spawnCars.Invoke();

            cameraViewCars.Clear();

            cameraViewCarsRefresh = true;

            cameraViewCarsIndex = 0;
        }










    }

    void OnApplicationQuit()
    {
        Debug.Log("captured frames are: " + capturedFramesCount);
    }
}
