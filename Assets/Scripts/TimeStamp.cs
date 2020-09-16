using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class TimeStamp : MonoBehaviour
{

    public Text text;
    public GameObject cameraObject;
    public Camera cameraComponent;
    public string cameraState = "OFF";
    public string colliders = "";
    public GameObject movingObject;
    public float speed;


    private void Start()
    {
        cameraObject = this.gameObject;
        cameraComponent = cameraObject.GetComponent<Camera>();
    }
    void Update()
    {


        string monthVar = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss ");
        string frameCount = (Time.frameCount).ToString();
        string cameraStatus = cameraState;
        string camName = cameraObject.name;
        string FPS = (1f / Time.deltaTime).ToString("F1");
        string timeSinceStart = Time.time.ToString();



        if (movingObject != null)
        {
            speed = movingObject.GetComponent<Rigidbody>().velocity.magnitude;
        }

        if (cameraObject.GetComponent<CameraTriggerBoundingBox>() != null)
        {
            colliders = cameraObject.GetComponent<CameraTriggerBoundingBox>().colliderNum.ToString();
            string _text = string.Format("Name: {0}\nFPS: {1}\nFrame count: {2}\nTime since start: {3}s\nColliders present: {4}\nLatency: {5}ms", camName, FPS, frameCount, timeSinceStart, colliders, Time.deltaTime*1000);
            text.text = _text;
        }

        else
        {

            if (movingObject != null)
            {

                string _text = string.Format("Name: {0}\nFPS: {1}\nFrame count: {2}\nTime since start: {3}s\nSpeed: {4}units/s\nLatency: {5}ms", camName, FPS, frameCount, timeSinceStart, speed, Time.deltaTime * 1000);
                text.text = _text;

            }

            else
            {

                string _text = string.Format("Name: {0}\nFPS: {1}\nFrame count: {2}\nTime since start: {3}s\nLatency: {4}ms", camName, FPS, frameCount, timeSinceStart, Time.deltaTime * 1000);
                text.text = _text;

            }



        }





    }

}

