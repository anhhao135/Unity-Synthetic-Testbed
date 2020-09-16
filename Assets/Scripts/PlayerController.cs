using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public NavMeshAgent navmeshagent;
    public List<Waypoint> patrolPoints = new List<Waypoint>();
    public Waypoint targetWaypoint;
    public float reachRadius = 1f;

    void Start()
    {
        navmeshagent = this.GetComponent<NavMeshAgent>();

    }



    // Update is called once per frame
    void Update()
    {
        

        

        /*

      targetWaypoint = patrolPoints[currentIndex];

      navmeshagent.SetDestination(targetWaypoint.transform.position);







      Vector3 distance = transform.position - targetWaypoint.transform.position;

      if (distance.magnitude < reachRadius)
      {
          currentIndex++;
      }

      if (currentIndex == patrolPoints.Count)
      {
          currentIndex = 0;
      }

  */

    }


}
