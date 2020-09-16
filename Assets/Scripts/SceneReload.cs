using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReload : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find("SceneReloader") != null && Time.frameCount > 1)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Debug.Log("start again");
            DontDestroyOnLoad(this.gameObject);
        }




    }

    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("r"))
        { //If you press R
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

 

    }
}