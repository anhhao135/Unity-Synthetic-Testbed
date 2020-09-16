using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOccluder : MonoBehaviour
{
    // Start is called before the first frame update

    public LayerMask occludeLayers;
    public List<GameObject> previousFrameObjects;
    public float occludeRadius = 8f;
    public Collider[] hitColliders;
    public CarSpawner carSpawner;
    public List<GameObject> spawnedCars;
    void Start()
    {
        spawnedCars = carSpawner.spawnedCars;
    }

    // Update is called once per frame
    void Update()
    {
        hitColliders = Physics.OverlapSphere(transform.position, occludeRadius, occludeLayers, QueryTriggerInteraction.Collide);



        foreach (Collider hitCollider in hitColliders)
        {

            GameObject parentObject = hitCollider.transform.parent.gameObject;

            foreach(GameObject car in spawnedCars)
            {
                if (car == parentObject)
                {
                    car.SetActive(false);
                }

                else
                {
                    car.SetActive(true);
                }
            }

            

            if (hitCollider.transform.parent.gameObject != null)
            {
                Renderer[] renderers = hitCollider.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                }

                previousFrameObjects.Add(hitCollider.transform.parent.gameObject);
            }

            else
            {
                Renderer[] renderers = hitCollider.gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                }

                previousFrameObjects.Add(hitCollider.gameObject);
            }
            
        }
    }
}
