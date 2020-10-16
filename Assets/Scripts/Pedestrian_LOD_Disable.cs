using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian_LOD_Disable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                continue;
            }

            children[i].gameObject.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
