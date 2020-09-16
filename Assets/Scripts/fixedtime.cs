using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixedtime : MonoBehaviour
{


    public float Timescaleforsonia = 1f;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        Time.captureDeltaTime = 0.0166666666666667f;
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {

        
        Time.timeScale = Timescaleforsonia;
        Debug.Log(i++);
        Debug.Log(Time.realtimeSinceStartup);
        
    }
}
