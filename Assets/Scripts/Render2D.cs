using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Render2D : MonoBehaviour
{

    public RenderTexture renderTexture;
    public Texture2D tex2d;
    public int outputWidth = 80;
    public int outputHeight = 45;
    public int densityFactor = 1;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //renderTexture.width = outputWidth*densityFactor;
        //renderTexture.height = outputHeight*densityFactor;
        tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();



    }
}
