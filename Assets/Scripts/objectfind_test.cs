using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectfind_test : MonoBehaviour
{
    // Start is called before the first frame update


    public Object[] meshRenderers;
    public Object[] skinRenderers;
    void Start()
    {

        /*
        meshRenderers = FindObjectsOfType(typeof(MeshRenderer));

        foreach (Object meshRenderer in meshRenderers)
        {
            MeshRenderer mr = (MeshRenderer)meshRenderer;
            MeshCollider mc = mr.gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
        }

        skinRenderers = FindObjectsOfType(typeof(SkinnedMeshRenderer));
        foreach (Object skinRenderer in skinRenderers)
        {
            SkinnedMeshRenderer mr = (SkinnedMeshRenderer)skinRenderer;
            skinnedmeshupdatetest mc = mr.gameObject.AddComponent<skinnedmeshupdatetest>();

        }

        */
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        try
        {
            if (Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime) == 2)
            {

                Debug.Log("run");
                meshRenderers = FindObjectsOfType(typeof(MeshRenderer));

                foreach (Object meshRenderer in meshRenderers)
                {
                    MeshRenderer mr = (MeshRenderer)meshRenderer;

                    if (mr.gameObject.GetComponent<MeshCollider>() == null && mr.gameObject.GetComponent<BoxCollider>() == null)
                    {
                        MeshCollider mc = mr.gameObject.AddComponent<MeshCollider>();
                        mc.convex = false;
                        mc.isTrigger = true;
                    }
                        
                    if (mr.gameObject.GetComponent<MeshCollider>() != null)
                    {
                        mr.gameObject.GetComponent<MeshCollider>().convex = true;
                    }

                }

                skinRenderers = FindObjectsOfType(typeof(SkinnedMeshRenderer));
                foreach (Object skinRenderer in skinRenderers)
                {
                    SkinnedMeshRenderer mr = (SkinnedMeshRenderer)skinRenderer;
                    skinnedmeshupdatetest mc = mr.gameObject.AddComponent<skinnedmeshupdatetest>();

                }
            }
        }

        catch (System.Exception e)
        {
            
        }
            


    }
}
