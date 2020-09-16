using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[ExecuteInEditMode]



//behaviour which should lie on the same gameobject as the main camera
public class PostProcessCamera : MonoBehaviour
{
    public Camera cam;

    //material that's applied when doing postprocessing
    [SerializeField]
    private Material postprocessMaterial;

    //method which is automatically called by unity after the camera is done rendering
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture
        Graphics.Blit(source, destination, postprocessMaterial);
    }

    void Start()
    {
        cam = this.GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }
}

