using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class AutoCarLight : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject EnviroSkyManager;

    void Start()
    {

        EnviroSkyManager = GameObject.Find("Enviro Sky Manager");
        
    }

    // Update is called once per frame
    void Update()
    {

        int currentHour;

        if (EnviroSkyManager != null)
        {
            currentHour = EnviroSkyManager.GetComponentInChildren<EnviroSkyMgr>().Time.Hours;

            if (currentHour < 7 || currentHour > 16)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }

            else
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        
    }
}
