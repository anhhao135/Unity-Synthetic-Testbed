using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carmovetest : MonoBehaviour
{

    Rigidbody body;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }
}


