using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControlEngineFlame : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] Transform[] engines;
    [SerializeField] float speed = 1f;
    private float minEngineScaleZ;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (engines[0] != null)
        {
            minEngineScaleZ = engines[0].transform.localScale.z;
        }
    }
    
    void Update()
    {
        ControlEngineFlameLength();
    }
    void ControlEngineFlameLength()
    {
        foreach (Transform engine in engines)
        {
            Vector3 engineScale = engine.localScale;
            engineScale.z = (rb.velocity.z + Input.GetAxis("Vertical")) * speed + minEngineScaleZ;
            if (engineScale.z < minEngineScaleZ)
            {
                engineScale.z = minEngineScaleZ;
            }
            engine.localScale = engineScale;
            
        }
    }
}
