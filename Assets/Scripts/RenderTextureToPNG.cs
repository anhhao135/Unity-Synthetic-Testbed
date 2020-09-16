using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureToPNG : MonoBehaviour
{

    public RenderTexture rt;
    public string savePath;

    public void Update()
    {

        if (Time.frameCount % 1 == 0)
        {
            SaveTexture();
        }

    }
    // Use this for initialization
    public void SaveTexture()
    {
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
