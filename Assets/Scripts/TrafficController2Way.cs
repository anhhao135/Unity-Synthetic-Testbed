using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController2Way : MonoBehaviour
{


    public GameObject way1Block;
    public GameObject way2Block;
    public float period;
    public float TimeCurrent;

    // Start is called before the first frame update
    void Start()
    {
        TimeCurrent = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (TimeCurrent < period)
        {
            way1Block.SetActive(true);
            way2Block.SetActive(false);
        }

        if (TimeCurrent > period && TimeCurrent < 2 * period)
        {
            way1Block.SetActive(false);
            way2Block.SetActive(true);
        }

        if (TimeCurrent > 2 * period)
        {
            TimeCurrent = 0f;
        }

        TimeCurrent += Time.deltaTime;
        
    }

}
