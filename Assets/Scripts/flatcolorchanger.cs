using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flatcolorchanger : MonoBehaviour
{
    // Start is called before the first frame update


    public SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 5 == 0)
        {


            sr.material.SetColor("_FlatColor", Random.ColorHSV());
            /*
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_Color", Random.ColorHSV());
            sr.SetPropertyBlock(mpb);
            */
        }
    }
}
