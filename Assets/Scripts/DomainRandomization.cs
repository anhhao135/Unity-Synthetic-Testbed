
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DomainRandomization : MonoBehaviour
{
    public Object[] flyingDistractionObjects;
    public List<Texture> randomTextures = new List<Texture>();
    public List<GameObject> activeDistractions = new List<GameObject>();
    public int minDistractions = 5;
    public int maxDistractions = 20;
    public float minDistractionScale = 0.5f;
    public float maxDistractionScale = 1.5f;
    private Camera camera;
    public GameObject pointLightPrefab;

    public int minLights = 5;
    public int maxLights = 30;
    public List<GameObject> activeLights = new List<GameObject>();

    public PostProcessVolume postProcessVolume;

    public bool toggleDistractions = false;
    public bool toggleLights = false;

    
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        flyingDistractionObjects = Resources.LoadAll("primitive_prefab");

    }

    // Update is called once per frame
    void Update()
    {

        //postProcessVolume.profile.GetSetting<MotionBlur>().shutterAngle.value = Random.Range(0, 50f);
        //this.camera.fieldOfView = Random.Range(50f, 70f);



        if (toggleDistractions == true)
        {
            foreach (GameObject distraction in activeDistractions)
            {
                Destroy(distraction.GetComponent<Renderer>().material);
                Destroy(distraction);
            }

            activeDistractions.Clear();
            activeDistractions.TrimExcess();


            int numberOfDistractions = Random.Range(minDistractions, maxDistractions);

            for (int i = 0; i < numberOfDistractions; i++)
            {
                int index = Random.Range(0, flyingDistractionObjects.Length);
                Vector3 p = camera.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(5, 30f)));
                GameObject instantiatedDistraction = Instantiate((GameObject)flyingDistractionObjects[index], p, Random.rotation);
                instantiatedDistraction.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(Random.Range(0, 5f), Random.Range(0, 2f)));
                instantiatedDistraction.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(Random.Range(0, 1f), Random.Range(0, 1f)));
                float scaleAmount = Random.Range(minDistractionScale, maxDistractionScale);
                instantiatedDistraction.transform.localScale *= scaleAmount;
                activeDistractions.Add(instantiatedDistraction);


            }

            
        }


        

        if (toggleLights == true)
        {
            foreach (GameObject light in activeLights)
            {
                Destroy(light);
            }

            activeLights.Clear();

            activeLights.TrimExcess();


            int numberOfLights = Random.Range(minLights, maxLights);

            for (int i = 0; i < numberOfLights; i++)
            {

                Vector3 p = camera.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(1f, 40f)));
                GameObject instantiatedLight = Instantiate(pointLightPrefab, p, Random.rotation);
                instantiatedLight.GetComponent<Light>().color = Random.ColorHSV();
                instantiatedLight.GetComponent<Light>().intensity = Random.Range(0, 10f);
                instantiatedLight.GetComponent<Light>().range = Random.Range(5f, 30f);
                activeLights.Add(instantiatedLight);

            }

        }


    }
}
