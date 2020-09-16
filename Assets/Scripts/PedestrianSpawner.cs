using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PedestrianSpawner : MonoBehaviour
{

    public List<GameObject> pedestrianList = new List<GameObject>();
    public GameObject rootWaypoint;
    private int waypointCount;
    public int spawnCount;
    public Transform centerSpawnPoint;
    public float centerSpawnDistance = 60f;
    public List<GameObject> existingPedestrians;
    public bool toggleManualSpawn = false;
    public GameObject pedSpritePrefab;
    public bool togglePedSpriteSpawn = false;
    public bool toggleOnlyPedSpriteSpawn = false;
    public float pedestrianSpawnRandomnessConstant; //determines randomness in spawn vector

    // Start is called before the first frame update
    public void Start()
    {

        if (toggleManualSpawn == true)
        {
            SpawnPedestrians();
        }


    }

    // Update is called once per frame
    public void Update()
    {
        if (Time.frameCount == 3)
        {
            DestroyPedestrians();
            SpawnPedestrians();
        }


    }

    public void SpawnPedestrians()
    {



        waypointCount = rootWaypoint.transform.childCount;

        List<Transform> nearbyWaypoints = new List<Transform>();

        foreach (Transform child in rootWaypoint.transform)
        {

            if (Vector3.Magnitude(centerSpawnPoint.position - child.position) < centerSpawnDistance)
            {
                nearbyWaypoints.Add(child);
            }

        }

        if (nearbyWaypoints.Count > 0)
        {
            for (int i = 0; i < spawnCount; i++)
            {


                int index = Random.Range(0, nearbyWaypoints.Count);


                Vector3 randomVector = nearbyWaypoints[index].position;

                float number;

                

                if (togglePedSpriteSpawn == true)
                {
                    number = Random.Range(0, 100f);
                }

                else
                {
                    number = 100f;
                }

                if (toggleOnlyPedSpriteSpawn == true)
                {
                    number = 0;
                }

                

                GameObject newPedestrian;

                if (number > 50f)
                {
                    int pedestrianListIndex = Random.Range(0, pedestrianList.Count); //choose random person preset from lost
                    newPedestrian = Instantiate(pedestrianList[pedestrianListIndex], randomVector + new Vector3(Random.Range(-pedestrianSpawnRandomnessConstant, pedestrianSpawnRandomnessConstant), 0, Random.Range(-pedestrianSpawnRandomnessConstant, pedestrianSpawnRandomnessConstant)), Quaternion.identity); //spawn at waypoint; instantiate
                    newPedestrian.GetComponent<PedestrianControl>().rootWaypoint = rootWaypoint; //pass rootwaypoint to instantiated ped
                }

                else
                {
                    newPedestrian = Instantiate(pedSpritePrefab, randomVector + new Vector3(Random.Range(-pedestrianSpawnRandomnessConstant, pedestrianSpawnRandomnessConstant), 0, Random.Range(-pedestrianSpawnRandomnessConstant, pedestrianSpawnRandomnessConstant)), Quaternion.identity); //spawn at waypoint; instantiate
                }

     




                existingPedestrians.Add(newPedestrian);

                newPedestrian.name = "Person_" + i; //name with unique id


            }

            nearbyWaypoints.Clear();
        }

    }

    public void DestroyPedestrians()
    {

        Debug.Log("destroying");

        foreach (GameObject Pedestrian in existingPedestrians)
        {
            Destroy(Pedestrian);
        }

        existingPedestrians.Clear();
        existingPedestrians.TrimExcess();
    }


}
