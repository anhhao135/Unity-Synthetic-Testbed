using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guassian_Distribution : MonoBehaviour
{

    public GameObject plot_point;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void scatterPlot(float x_value, GameObject point)
    {
        GameObject new_point = GameObject.Instantiate(point);
        new_point.transform.position = new Vector3(0f, 0f, x_value);
    }

    // Update is called once per frame
    void Update()
    {

        
        float mean = 0f;
        float stdDev = 1000f;
        float u1 = Random.Range(0f, 1f); //uniform(0,1] random doubles
        float u2 = Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        

        //float randNormal = Random.Range(-30f, 30f);

        scatterPlot(randNormal, plot_point);
    }
}
