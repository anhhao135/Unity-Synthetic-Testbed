using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CameraCSV : MonoBehaviour
{

    private List<string[]> rowData = new List<string[]>();

    [SerializeField] Camera[] allCameras;

    [SerializeField] List<Camera> validCameras;



    // Use this for initialization
    void Start()
    {
        allCameras = null;
        allCameras = Camera.allCameras;
        validCameras = new List<Camera>();

        for (int i = 0; i < allCameras.Length; i++)
        {
            if (allCameras[i].gameObject.GetComponent<TimeStamp>() != null)
            {
                validCameras.Add(allCameras[i]);
            }

        }


        // Creating First row of titles manually..
        string[] rowDataTemp = new string[2 + validCameras.Count*2];
        rowDataTemp[0] = "Frame";
        rowDataTemp[1] = "Time since start (s)";

        for (int i = 0; i < validCameras.Count * 2; i++)
        {
            Camera currentCamera = validCameras[i/2];
            rowDataTemp[i + 2] = currentCamera.name + "STATE";
            i++;
            rowDataTemp[i + 2] = currentCamera.name + "COLLIDERS";
        }

        rowData.Add(rowDataTemp); //adds top label

    }

    private void Update()
    {
        
        string[] rowDataTemp = new string[2 + validCameras.Count*2];

        rowDataTemp[0] = (Time.frameCount).ToString();
        rowDataTemp[1] = Time.time.ToString();

        
        for (int i = 0; i < validCameras.Count * 2; i++)
        {
            Camera currentCamera = validCameras[i / 2];
            rowDataTemp[i + 2] = currentCamera.gameObject.GetComponent<TimeStamp>().cameraState;
            i++;
            rowDataTemp[i + 2] = currentCamera.gameObject.GetComponent<CameraTriggerBoundingBox>().colliderNum.ToString();

        }
        

        rowData.Add(rowDataTemp); //updates row

    
    }

    void OnApplicationQuit()
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            Debug.Log(i);
            sb.AppendLine(string.Join(delimiter, output[i]));
        }


        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();


        Debug.Log("Saved!");
    }


    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data.csv";
#elif UNITY_ANDROID
    return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
    return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
    return Application.dataPath +"/"+"Saved_data.csv";
#endif
    }



}









