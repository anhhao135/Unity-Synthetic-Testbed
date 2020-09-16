using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capsulemove : MonoBehaviour
{
    public bool togglefixed = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (togglefixed != false)
        {
            transform.Translate(transform.forward * 1f * Time.deltaTime);
        }

        else
        {
            transform.Translate(transform.forward * 1f * Time.fixedDeltaTime);
        }
        
    }
}
