using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject routes;
    public List<GameObject> carPreFabs = new List<GameObject>();
    public List<GameObject> realCarPrefabs = new List<GameObject>();
    public float densityProbability = 0.5f;
    public List<GameObject> spawnedCars;
    public Object[] patternsList;
    public bool toggleSpawnRealCarsOnly = false;
    public bool toggleSpawn5050Cars = false;

    // Start is called before the first frame update
    void Start()
    {

        patternsList = Resources.LoadAll("textures_patterns", typeof(Texture2D));

        if (toggleSpawnRealCarsOnly == true)
        {
            carPreFabs.Clear();
            carPreFabs = realCarPrefabs;
            Debug.Log("car list updated");
        }

        if (toggleSpawn5050Cars == true)
        {
            foreach (GameObject realcar in realCarPrefabs)
            {
                carPreFabs.Add(realcar);
            }
        }



        SpawnNewCars();




    }

    // Update is called once per frame
    void Update()
    {
        Resources.UnloadUnusedAssets();
    }

    public void DestroySpawnedCars()
    {
        foreach(GameObject car in spawnedCars)
        {
            Destroy(car);
        }

        spawnedCars.Clear();
        spawnedCars.TrimExcess();
    }

    public void SpawnNewCars()
    {
        foreach (Transform route in routes.transform)
        {
            foreach (Transform waypoint in route)
            {

                if (route.GetChild(route.childCount - 1) == waypoint)
                {
                    continue;
                }

                if (Random.Range(0, 1f) <= densityProbability)
                {
                    int index = Random.Range(0, carPreFabs.Count);
                    GameObject newCar = Instantiate(carPreFabs[index], waypoint);
                    spawnedCars.Add(newCar);
                    newCar.transform.position += newCar.transform.up * 4f;
                    newCar.transform.rotation = Quaternion.LookRotation(waypoint.forward, waypoint.up);

                    //Texture2D newPattern = (Texture2D)patternsList[Random.Range(0, patternsList.Length)];

                    //newPattern.hideFlags = HideFlags.HideAndDontSave;

                    Component[] renderers = newCar.GetComponentsInChildren<Renderer>();


                    
                    
                    

                    foreach (MeshRenderer renderer in renderers)
                    {
                        Material[] materials = renderer.materials;
                        
                        foreach (Material material_ in materials)
                        {
                            if (material_.GetFloat("_ZWrite") == 0)
                            {
                                continue;
                            }

                            
                            //material_.SetTexture("_MainTex", newPattern);
                            //material_.SetColor("_Color", Random.ColorHSV());
                            //float scale = Random.Range(0f, 5f);
                            //material_.SetTextureScale("_MainTex", new Vector2(scale, scale));
                            
                        }

                    }

                    


                    


                    

                    


                    RaycastHit hit;

                    if (Physics.Raycast(newCar.transform.position, newCar.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
                    {


                        float difference = hit.distance;

                        newCar.transform.position += Vector3.down * difference;

                    }

                }

            }
        }
    }

    void OnApplicationQuit()
    {
        Resources.UnloadUnusedAssets();
    }
}
