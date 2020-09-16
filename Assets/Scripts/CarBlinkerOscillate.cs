using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBlinkerOscillate : MonoBehaviour
{

    public GameObject frontBlinker;
    public GameObject rearBlinker;
    private float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = Random.Range(0f, 5f);
    }

    // Update is called once per frame
    void Update()
    {


        currentTime += Time.deltaTime;

        float signal = Mathf.Sin(8 * currentTime);

        if (signal > 0)
        {
            frontBlinker.SetActive(true);
            rearBlinker.SetActive(true);
        }

        else
        {
            frontBlinker.SetActive(false);
            rearBlinker.SetActive(false);
        }
    }
}
