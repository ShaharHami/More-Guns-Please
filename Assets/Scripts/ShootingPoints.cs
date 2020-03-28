using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingPoints : MonoBehaviour
{
    public string name;
    public bool enabled;
    public Volley volley;
    [Header("Gizmo Colors")] 
    public Color lineColor;
    public Color pointColor;
    public abstract Vector3[] GetShootingPoints();
    private void Update()
    {
        if (volley != null)
        {
            volley.SetShootingPoints(GetShootingPoints());
        }
    }
}
