using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRandomMaterial : MonoBehaviour
{

    public List<Material> materialsList = new List<Material>();
    public Material selectedMaterial;
    public MeshRenderer mr;
    // Start is called before the first frame update
    void OnEnable()
    {
        selectedMaterial = materialsList[Random.Range(0, materialsList.Count)];
        mr = GetComponent<MeshRenderer>();
        mr.material = selectedMaterial;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
