using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skinnedmeshupdatetest : MonoBehaviour
{


    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;

    // Start is called before the first frame update
    void Start()
    {

        
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        collider = this.gameObject.AddComponent<MeshCollider>();
        collider.convex = false;

        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;

    }

    // Update is called once per frame
    void Update()
    {


    }
}

