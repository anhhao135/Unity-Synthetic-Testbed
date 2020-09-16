using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PedestrianControl : MonoBehaviour


{
    //public ThirdPersonCharacter character;

    public GameObject rootWaypoint;

    public NavMeshAgent navmeshagent;

    public float reachRadius = 1.5f;

    //[SerializeField] Waypoint targetWaypoint;

    private bool travelling = false;

    public float minSpeed;
    public float maxSpeed;

    private float speed;

    private Vector3 targetWaypoint;

    private List<Animator> animators = new List<Animator>();

    public float animatorSpeedScaleConstant = 0.7f; //dont mess with this; it is tested

    public float minHeight;
    public float maxHeight;

    private float height;

    private NavMeshPath path;

    //private Vector3 lastFramePosition;

    //private Vector3 currentFramePosition;

    //private Vector3 velocity;

    //private float realSpeed;

    //private Vector3 Distance;

    //public bool infected = false;

    //[SerializeField] Collider[] hitNPC;

    //public float infectRadius = 2f;

    //int layerId = 8;
    //int layerMask;


    // Start is called before the first frame update
    void Start()
    {
        minHeight = 0.85f;
        maxHeight = 1.15f;

        minSpeed = 0.8f;
        maxSpeed = 2.5f;

        //rootWaypoint = GameObject.Find("Pedestrian Waypoints"); //this might break, instead pedestrianSpawner script will pass the universal ped root waypoint value
        speed = Random.Range(minSpeed, maxSpeed);
        navmeshagent = GetComponent<NavMeshAgent>();
        navmeshagent.speed = speed;
        height = Random.Range(minHeight, maxHeight);
        //transform.localScale = new Vector3(1, height, 1);
        //transform.localScale = transform.localScale * height; //height acts funny, dont play with it


        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Animator>() != null)
            {
                animators.Add(child.gameObject.GetComponent<Animator>());
            }
            
        }

        path = new NavMeshPath();





        //navmeshagent.updateRotation = false;
        //Travelling = false;


        //layerMask = 1 << layerId;


    }

    // Update is called once per frame


    private void Update()
    {
        if (Time.frameCount % 40 == 0 && Random.Range(0f,1f) < 0.1f)
        {
            travelling = false;
        }
    }
    private void LateUpdate()
    {

        if (travelling == false)
        {

            path.ClearCorners();

            GameObject targetWaypointObject;

            targetWaypointObject = rootWaypoint.transform.GetChild(Random.Range(0, rootWaypoint.transform.childCount)).gameObject;

            targetWaypoint = targetWaypointObject.transform.position;

            while (targetWaypointObject.activeSelf == false)
            {
                targetWaypointObject = rootWaypoint.transform.GetChild(Random.Range(0, rootWaypoint.transform.childCount)).gameObject;
            }

            NavMesh.CalculatePath(transform.position, targetWaypointObject.transform.position, NavMesh.AllAreas, path);

            navmeshagent.SetPath(path);

            //navmeshagent.SetDestination(targetWaypoint = targetWaypointObject.transform.position);

            travelling = true;

        }

        if (Vector3.Magnitude(transform.position - targetWaypoint) < reachRadius)
        {
            travelling = false;

            foreach (Animator animator in animators)
            {
                animator.SetBool("walk", false);
            }


        }

        if (travelling == true)
        {
            foreach (Animator animator in animators)
            {
                animator.speed = speed * animatorSpeedScaleConstant;
                animator.SetBool("walk", true);
            }
        }
    }

}
