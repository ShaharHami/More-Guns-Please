using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Volley : MonoBehaviour
{
    public float volleyAngle;
    public int numberOfShots;
    public float shotsInterval;
    public float volleyShotsInterval;
    public float distanceFromOrigin;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var v2 = volleyAngle / 2;
        Debug.DrawLine(transform.position, transform.position + transform.forward * 20);
    }

    public void ShootVolley()
    {
        for (int i = 0; i < numberOfShots; i++)
        {
            
        }
    }
}