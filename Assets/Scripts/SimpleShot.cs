using System;
using System.Collections;
using UnityEngine;

public class SimpleShot : MonoBehaviour
{
    public float speed;
    public bool goStraight;
    public bool scatterShot;
    private Coroutine coroutine;

    private void OnEnable()
    {
        if (goStraight)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speed * transform.forward;
    }
}
