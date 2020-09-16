using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class cameramatrixtest : MonoBehaviour
{


    public Camera cam;
    public Transform objectrack;
    public Transform Bobject;

    Matrix4x4 cameraTRS;
    Matrix4x4 BTRS;
    Matrix4x4 originalProjection;

    private DirectoryInfo sessionDir; //session's directory
    // Start is called before the first frame update
    void Start()
    {

        cam = GetComponent<Camera>();
        cameraTRS =  Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, cam.transform.localScale);
        BTRS = Matrix4x4.TRS(Bobject.transform.position, Bobject.transform.rotation, Bobject.transform.localScale);
        originalProjection = cam.projectionMatrix;

        sessionDir = Directory.CreateDirectory(string.Format("Session_{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now)); //create session directory unique to start system time




    }

    // Update is called once per frame
    void Update()
    {
        Vector4 capsuleWorld = objectrack.transform.position;

        Debug.Log(cameraTRS);
        //Vector4 capsuleRelativetoCam = cameraTRS.inverse.MultiplyPoint(capsuleWorld);
        //Vector3 localB = cameraTRS.MultiplyPoint(capsuleRelativetoCam);
        //localB = BTRS.inverse.MultiplyPoint(localB);


        //capsuleRelativetoCam.z = -capsuleRelativetoCam.z;

        //Vector3 sonia = manualWorldToScreenPoint(objectrack.position);

        //Debug.Log(manualWorldToScreenPoint(objectrack.position));



        //Debug.Log("local A" + capsuleRelativetoCam);
        //Debug.Log(cam.worldToCameraMatrix.MultiplyPoint(objectrack.position));
        //Debug.Log("local B" + localB);
        //Debug.Log(cam.worldToCameraMatrix);

        //Vector4 capsuleClip = cam.projectionMatrix.MultiplyPoint(capsuleRelativetoCam);
        //Vector2 screenpos = new Vector2(capsuleClip.x * cam.pixelWidth, capsuleClip.y *);
        //Debug.Log(capsuleClip);

        //Matrix4x4 p = originalProjection;
        //Debug.Log(originalProjection);

    }


    Vector2 manualWorldToScreenPoint(Vector3 wp)
    {
        // calculate view-projection matrix
        Matrix4x4 mat = cam.projectionMatrix * cam.worldToCameraMatrix;

        // multiply world point by VP matrix
        Vector4 temp = mat * new Vector4(wp.x, wp.y, wp.z, 1f);

        Vector4 tempNDC = temp / temp.w;

        Vector2 screenSpace;

        screenSpace.x = (cam.pixelWidth / 2f)*(1 + tempNDC.x);
        screenSpace.y = (cam.pixelHeight / 2f) * (1 + tempNDC.y);

        return screenSpace;

        /*

        Debug.Log(temp / temp.w);

        if (temp.w == 0f)
        {
            // point is exactly on camera focus point, screen point is undefined
            // unity handles this by returning 0,0,0
            return Vector3.zero;
        }
        else
        {
            // convert x and y from clip space to window coordinates
            temp.x = (temp.x / temp.w + 1f) * .5f * cam.pixelWidth;
            temp.y = (temp.y / temp.w + 1f) * .5f * cam.pixelHeight;
            return new Vector3(temp.x, temp.y, wp.z);
        }
        */



    }
}
