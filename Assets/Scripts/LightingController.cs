using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingController : MonoBehaviour
{

    public Light worldSun;
    public Material day;
    public Material sunset;
    public Material night;
    public Quaternion sunRotation;
    public Gradient sunGradient;
    [Range(0f, 24f)] public float timeOfDay;
    public float sunIntensity;
    public bool toggleSine;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleSine == true)
        {
            timeOfDay = 24 * Mathf.Abs(Mathf.Sin(0.1f * Time.time));
        }


        float timeOfDayToKey = timeOfDay * (1f / 24f);
        worldSun.color = sunGradient.Evaluate(timeOfDayToKey);
        worldSun.transform.localRotation = Quaternion.Euler(new Vector3((timeOfDayToKey * 360f) - 90f, 170f, 0f));
        worldSun.intensity = sunGradient.Evaluate(timeOfDayToKey).a * 2;
        sunIntensity = sunGradient.Evaluate(timeOfDayToKey).a * 1.5f;

        if (timeOfDay > 8 && timeOfDay < 13)
        {
            RenderSettings.skybox = day;
        }

        if (timeOfDay > 13 && timeOfDay < 19)
        {
            RenderSettings.skybox = sunset;
        }

        if (timeOfDay > 19 || timeOfDay < 8)
        {
            RenderSettings.skybox = night;
        }

    }
}
