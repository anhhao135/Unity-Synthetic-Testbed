using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navmesh_Test_character : MonoBehaviour
{

    public Animator anim;
    public NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(194, 341, -200));
        anim.SetBool("walk", true);
        //GetComponent<NavMeshAgent>().Move(transform.forward * Time.deltaTime);

    }
}
