using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshRendererColliderAdd : MonoBehaviour
{


    //add this to the rootobject of pedestrian
    // Start is called before the first frame update
    void Start()
    {
        SkinnedMeshRenderer[] renderers;

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.gameObject.AddComponent<skinnedmeshupdatetest>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
