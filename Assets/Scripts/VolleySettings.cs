using System;
using UnityEngine;

[System.Serializable]
public class VolleySettings
{
    public string name;
    public GameObject shotPrefab;
    [Range(0f, 360f)] public float volleyAngle;
    public int numberOfShots;
    public float distanceFromCenter;
    public float centerOffsetX, centerOffsetZ;
    public float volleyInterval;
    public float shotsInterval;
    public bool randomBarrel;
    public bool sequential;
    public bool ltr;
    public bool alternating;
}
