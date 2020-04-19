using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AngularShootingPoints : ShootingPoints
{
    [Header("Angular Shot Settings")]
    public int shootingPoints;

    [Range(0f, 360f)] public float shootingAngle;
    public float distanceFromCenter;
    public float centerOffsetX, centerOffsetZ;

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
        for (int i = 0; i < shootingPoints; i++)
        {
            var a = GetAngles(i);
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position + offset, a);
            Gizmos.color = pointColor;
            Gizmos.DrawSphere(a, 0.25f);
        }
    }

    public override Vector3[] GetShootingPoints()
    {
        Vector3[] shootingPoints = new Vector3[this.shootingPoints];
        for (int i = 0; i < this.shootingPoints; i++)
        {
            shootingPoints[i] = GetAngles(i);
        }

        return shootingPoints;
    }

    public override void UpgradeHealth(float n)
    {
    }

    protected override void UpgradeChildCount(float n)
    {
        if (!enabled)
        {
            enabled = true;
        }
        else
        {
            if (shootingPoints >= maxShootingPoints) return;
            shootingPoints += (int) n;
            shootingAngle += n * ((maxShootingPoints - shootingPoints) * 0.5f);
        }
        MaxedOut = shootingPoints >= maxShootingPoints;
    }
    private Vector3 GetAngles(int posNum)
    {
        Vector3 offset = new Vector3(
            centerOffsetX,
            0,
            centerOffsetZ
        );
        float angle = (posNum + .5f) * ((Mathf.PI * 2f) * (shootingAngle / 360)) / shootingPoints;
        angle += ((Mathf.PI * (-1 / (360 / shootingAngle))) + (Mathf.PI * 0.5f)) -
                 (transform.eulerAngles.y * Mathf.Deg2Rad);
        return transform.position + offset +
               (new Vector3(Mathf.Cos(angle) * distanceFromCenter, 0, Mathf.Sin(angle) * distanceFromCenter));
    }
}