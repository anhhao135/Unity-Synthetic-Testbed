using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skinnedmeshupdatetest : MonoBehaviour
{


    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;

    // Start is called before the first frame update

    void update_skin()
    {

        if (this.gameObject.GetComponent<MeshCollider>() != null)
        {
            Destroy(this.gameObject.GetComponent<MeshCollider>());
        }

        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        collider = this.gameObject.AddComponent<MeshCollider>();
        collider.convex = false;

        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }
    void Start()
    {
        update_skin();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void FixedUpdate()
    {
        if (Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime) % 200 == 0)
        {
            update_skin();
        }
    }
}

