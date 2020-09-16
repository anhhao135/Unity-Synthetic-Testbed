using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjExporter;

public class NearRangeLidarTest : MonoBehaviour
{

    public Mesh mesh;


    // Start is called before the first frame update
    void Start()
    {


        
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.frameCount % 50 == 0)
        {
            ObjExporter.ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "test_" + Time.frameCount.ToString() + ".obj");

            Debug.Log("obj written");
        }




    }

    private void OnApplicationQuit()
    {

    }
}
