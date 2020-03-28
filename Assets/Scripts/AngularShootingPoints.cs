using System;
using UnityEngine;

public class AngularShootingPoints : ShootingPoints
{
    [Header("Circular Shot Settings")]
    public int numberOfPoints;
    [Range(0f, 360f)] public float shootingAngle;
    public float distanceFromCenter;
    public float centerOffsetX, centerOffsetZ;
    

    private void OnEnable()
    {
        if (volley != null)
        {
            volley.SetShootingPoints(GetShootingPoints());
        }
    }

    private void OnDrawGizmos()
    {
        if (volley != null)
        {
            volley.SetShootingPoints(GetShootingPoints());
        }
        Vector3 offset = new Vector3(
            centerOffsetX,
            0,
            centerOffsetZ
            );
        for (int i = 0; i < numberOfPoints; i++)
        {
            var a = GetAngles(i);
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position + offset, a);
            Gizmos.color = pointColor;
            Gizmos.DrawCube(a, Vector3.one / 2);
        }
    }

    public override Vector3[] GetShootingPoints()
    {
        Vector3[] shootingPoints = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            shootingPoints[i] = GetAngles(i);
        }
        return shootingPoints;
    }
    private Vector3 GetAngles(int posNum)
    {
        Vector3 offset = new Vector3(
            centerOffsetX,
            0,
            centerOffsetZ
        );
        float angle = (posNum + .5f) * ((Mathf.PI * 2f) * (shootingAngle / 360)) / numberOfPoints;
        angle += ((Mathf.PI * (-1 / (360 / shootingAngle))) + (Mathf.PI * 0.5f));
        return transform.position + offset +
               (new Vector3(Mathf.Cos(angle) * distanceFromCenter, 0, Mathf.Sin(angle) * distanceFromCenter));
    }
}
