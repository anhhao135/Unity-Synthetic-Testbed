using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TurnTheGameOn.SimpleTrafficSystem;
using UnityEngine;
using UnityEngine.Events;

public class AutomaticCameraMove : MonoBehaviour
{

    public Transform rootCameraWaypoint;
    public float rotationIncrement;
    public List<Transform> cameraWaypoints;
    private int cameraWaypointListIndex = 0;
    [SerializeField] float i;
    public UnityEvent spawnPedestrians;
    public UnityEvent destroyPedestrians;
    public UnityEvent onSceneChange;
    public int revolutions = 1;
    public float cameraAngleRandomization = 10f;
    private static System.Random rng = new System.Random();
    public bool toggleRandomCamRotation = false;
    public bool toggleRandomCamPosition = false;
    public bool toggleCameraRotation = false;
    public GameObject currentCameraCar;
    bool foundcar = false;
    private List<Transform> unshuffledCameraWaypoints;

    // Start is called before the first frame update


    void Shuffle(List<Transform> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Transform value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void Start()
    {
        Debug.Log("Start: " + System.DateTime.Now);

        foreach (Transform child in rootCameraWaypoint)
        {
            cameraWaypoints.Add(child);
        }

        unshuffledCameraWaypoints = cameraWaypoints;

        Shuffle(cameraWaypoints);

        transform.position = cameraWaypoints[cameraWaypointListIndex].position;

        GameObject.Find("SpawnCenter").transform.position = cameraWaypoints[cameraWaypointListIndex].position;


        spawnPedestrians.Invoke();


        currentCameraCar = null;

        i = 0;

        
    }

    // Update is called once per frame
    void Update()
    {

        


        
        

        if (toggleCameraRotation == true)
        {

            if (toggleRandomCamPosition == true)
            {
                transform.position = cameraWaypoints[cameraWaypointListIndex].position + Random.insideUnitSphere * 3f;
            }

            else
            {
                transform.position = cameraWaypoints[cameraWaypointListIndex].position;
            }


            if (cameraWaypointListIndex < cameraWaypoints.Count)
            {

                if (i < revolutions * 360f)
                {




                    if (toggleRandomCamRotation == true)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(-cameraAngleRandomization * 2f, cameraAngleRandomization * 5f), i, Random.Range(-cameraAngleRandomization, cameraAngleRandomization)));
                    }

                    else
                    {
                        transform.Rotate(Vector3.up * rotationIncrement, Space.World);
                    }

                    i = i + rotationIncrement;

                }

                else
                {
                    destroyPedestrians.Invoke();
                    i = 0;
                    cameraWaypointListIndex++;
                    //transform.position = cameraWaypoints[cameraWaypointListIndex].position;

                    GameObject.Find("SpawnCenter").transform.position = cameraWaypoints[cameraWaypointListIndex].position;

                    spawnPedestrians.Invoke();




                }

            }

            else
            {
                Debug.Log("End: " + System.DateTime.Now);
                UnityEditor.EditorApplication.isPlaying = false;
            }

        }

        else
        {



            if (cameraWaypointListIndex < cameraWaypoints.Count)
            {






                if (i < 300)
                {
                    i++;
                    
                }

                else
                {
                    destroyPedestrians.Invoke();
                    i = 0;
                    cameraWaypointListIndex++;
                    //transform.position = cameraWaypoints[cameraWaypointListIndex].position;

                    GameObject.Find("SpawnCenter").transform.position = cameraWaypoints[cameraWaypointListIndex].position;

                    spawnPedestrians.Invoke();

                    



                }


            }

            else
            {
                Debug.Log("End: " + System.DateTime.Now);
                UnityEditor.EditorApplication.isPlaying = false;
            }



        }

    }




}
