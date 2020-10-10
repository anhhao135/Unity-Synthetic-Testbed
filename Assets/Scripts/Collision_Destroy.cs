using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Destroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.GetComponentInChildren<Collision_Destroy>() != null || collision.collider.transform.root.tag == "Person")
        {
            //Debug.Log(collision.collider.name);
            collision.collider.transform.root.gameObject.SetActive(false);
            transform.root.gameObject.SetActive(false);

        }
        
    }
}
