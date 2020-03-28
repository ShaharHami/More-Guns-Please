using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShootingPoint
{
    public Transform shootingPoint;
    public bool enabled;
}
public class ManualShootingPoints : ShootingPoints
{
    public List<ShootingPoint> shootingPoints;
    private Vector3[] points;
    private void OnDrawGizmos()
    {
        points = GetShootingPoints();
        for (int i = 0; i < points.Length; i++)
        {
            var a = points[i];
            Gizmos.color = pointColor;
            Gizmos.DrawCube(a, Vector3.one / 2);
        }
    }

    public override Vector3[] GetShootingPoints()
    {
        return (from sp in shootingPoints where sp.enabled select sp.shootingPoint.position).ToArray();
    }
}
