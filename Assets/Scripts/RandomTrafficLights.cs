using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrafficLights : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> trafficlights;

    void Start()
    {
        foreach (Transform child in transform)
        {
            trafficlights.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject trafficlight in trafficlights)
        {
            trafficlight.SetActive(false);
        }

        int randomindex = Random.Range(0, trafficlights.Count);
        trafficlights[randomindex].SetActive(true);
    }
}
