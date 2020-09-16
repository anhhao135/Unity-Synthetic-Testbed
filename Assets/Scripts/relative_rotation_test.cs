using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relative_rotation_test : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform rotObj;

    public Vector3 rotationrelative;

    public Vector3 positionrelative;
    void Start()
    {




    }

    // Update is called once per frame
    void Update()
    {


        Quaternion relative = Quaternion.Inverse(transform.rotation) * rotObj.transform.rotation;



        rotationrelative = relativeRotation(transform, rotObj);


        positionrelative = relativePosition(transform, rotObj);

    }


    Vector3 relativeRotation (Transform parentObject, Transform targetObject)
    {
        Quaternion relative = Quaternion.Inverse(parentObject.rotation) * targetObject.rotation;
        Vector3 rotationsonia = relative.eulerAngles;


        float rz = rotationsonia.y;

        if (rz == 0)
        {
            rotationsonia.y = rz;
        }

        if (rz > 0 && rz <= 180f)
        {
            rotationsonia.y = -rz;
        }

        if (rz > 180f && rz < 360f)
        {
            rotationsonia.y = 360f - rz;
        }


        return rotationsonia;
    }

    Vector3 relativePosition (Transform parentObject, Transform targetObject)
    {
        return parentObject.InverseTransformPoint(targetObject.position);
    }
}
