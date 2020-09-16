using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BroadcastMessageTest : MonoBehaviour
{

    public bool Spawn_ = false;
    public bool Destroy_ = false;
    public UnityEvent spawnPedestrians;
    public UnityEvent destroyPedestrians;
    // Start is called before the first frame update
    void Start()
    {

        //Random.InitState(135);
        
    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            spawnPedestrians.Invoke();

            Spawn_ = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            destroyPedestrians.Invoke();

            Destroy_ = false;
        }

    }


}
