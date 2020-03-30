using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RotateMaterial : MonoBehaviour
{
    public Renderer renderer;
    public float scrollSpeed;
    private float sign;

    private void OnEnable()
    {
        sign = Random.Range(0f, 1f) > 0.5f ? Mathf.Sign(1) : Mathf.Sign(-1);
        scrollSpeed *= sign;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + (scrollSpeed * Time.deltaTime), 0));
        transform.rotation = rotation;
    }
}
