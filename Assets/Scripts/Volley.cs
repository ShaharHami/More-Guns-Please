using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Volley : MonoBehaviour
{
    public GameObject shotPrefab;
    public float volleyInterval;
    public float shotsInterval;
    public bool randomBarrel;
    public bool sequential;
    public bool ltr;
    public bool alternating;
    private Coroutine volleyCoroutine;
    private Coroutine shotsCoroutine;
    private List<Coroutine> coroutines;
    public Vector3[] shootingPoints;

    private void OnEnable()
    {
        coroutines = new List<Coroutine>();
    }

    private void OnDisable()
    {
        StopCoroutines();
    }

    public void SetShootingPoints(Vector3[] points)
    {
        shootingPoints = points;
    }
    public void PerformSimpleVolley(bool shoot)
    {
        if (!shoot)
        {
            StopCoroutines();
            return;
        }

        volleyCoroutine = StartCoroutine(ShootCoroutine());
        coroutines.Add(volleyCoroutine);
    }

    private void SingleVolley()
    {
        if (sequential)
        {
            shotsCoroutine = StartCoroutine(ShootVolleyCoroutine());
            coroutines.Add(shotsCoroutine);
        }
        else
        {
            if (shotsCoroutine != null)
            {
                StopCoroutine(shotsCoroutine);
            }

            ShootVolley();
        }

        if (alternating)
        {
            ltr = !ltr;
        }
    }

    private void StopCoroutines()
    {
        if (coroutines == null || coroutines.Count <= 0) return;
        foreach (var coroutine in coroutines.Where(coroutine => coroutine != null))
        {
            StopCoroutine(coroutine);
        }
    }

    private IEnumerator ShootVolleyCoroutine()
    {
        if (ltr)
        {
            for (int i = shootingPoints.Length - 1; i >= 0; i--)
            {
                PositionShot(i);
                yield return new WaitForSeconds(shotsInterval);
            }
        }
        else
        {
            for (int i = 0; i < shootingPoints.Length; i++)
            {
                PositionShot(i);
                yield return new WaitForSeconds(shotsInterval);
            }
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            SingleVolley();
            yield return new WaitForSeconds(volleyInterval);
        }
    }

    private void ShootVolley()
    {
        if (ltr)
        {
            for (int i = shootingPoints.Length - 1; i >= 0; i--)
            {
                PositionShot(i);
            }
        }
        else
        {
            for (int i = 0; i < shootingPoints.Length; i++)
            {
                PositionShot(i);
            }
        }
    }

    private void PositionShot(int shotNum)
    {
        Vector3 point = shootingPoints[randomBarrel ? Random.Range(0, shootingPoints.Length) : shotNum];
        Quaternion dir = Quaternion.LookRotation((point - transform.position), Vector3.up);
        GameObject obj = ObjectPooler.Instance.SpawnFromPool(shotPrefab.name, point, dir);
    }
}