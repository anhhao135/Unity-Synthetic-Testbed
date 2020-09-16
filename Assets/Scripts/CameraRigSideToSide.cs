using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigSideToSide : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 startLocalPosition;
    public float jiggleSpeed = 0.02f; //control frequency of triangle wave
    public float jigglePeaktoPeakMultiplier = 1f; //control amplitude of triangle wave
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        transform.localPosition = startLocalPosition + TriangleWave(Time.frameCount * jiggleSpeed) * Vector3.right * jigglePeaktoPeakMultiplier; //move side to side based on triangle wave.

        
    }

    public static float TriangleWave(float x)
    {
        return UnityEngine.Mathf.Abs((x % 4) - 2) - 1;
    }
}
