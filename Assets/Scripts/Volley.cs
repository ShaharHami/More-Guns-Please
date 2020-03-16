using System.Collections;
using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Volley : MonoBehaviour
{
    public GameObject shotPrefab;
    [Range(0f, 360f)] public float volleyAngle;
    public int numberOfShots;
    public float distanceFromCenter;
    public float volleyInterval;
    public float shotsInterval;
    public bool randomShot;
    public bool randomTiming;
    public bool ltr;
    public bool alternating;
    private Coroutine volleyCoroutine;
    private Coroutine shotsCoroutine;
    private List<Coroutine> coroutines;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < numberOfShots; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, GetAngles(i));
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(GetAngles(i), Vector3.one / 2);
        }
    }

    private void OnEnable()
    {
        FireWeapon.RegisterListener(PerformShot);
        coroutines = new List<Coroutine>();
    }

    private void OnDisable()
    {
        FireWeapon.UnregisterListener(PerformShot);
        StopCoroutines();
    }
    public void PerformShot(FireWeapon info = null)
    {
        if (info == null) return;
        PerformSimpleVolley(info.fire);
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
        if (randomTiming)
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
        foreach (Coroutine coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
    }

    private IEnumerator ShootVolleyCoroutine()
    {
        if (ltr)
        {
            for (int i = numberOfShots; i > 0; i--)
            {
                PositionShot(i);
                yield return new WaitForSeconds(shotsInterval);
            }
        }
        else
        {
            for (int i = 0; i < numberOfShots; i++)
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
            for (int i = numberOfShots; i > 0; i--)
            {
                PositionShot(i);
            }
        }
        else
        {
            for (int i = 0; i < numberOfShots; i++)
            {
                PositionShot(i);
            }
        }
    }

    private void PositionShot(int shotNum)
    {
        Vector3 pos;
        if (randomShot)
        {
            pos = GetAngles(Random.Range(0, numberOfShots));
        }
        else
        {
            pos = GetAngles(shotNum);
        }

        GameObject obj = ObjectPooler.Instance.SpawnFromPool(shotPrefab.name, pos, Quaternion.identity);
    }

    private Vector3 GetAngles(int posNum)
    {
        float angle = (posNum + .5f) * ((Mathf.PI * 2f) * (volleyAngle / 360)) / numberOfShots;
        angle = angle + ((Mathf.PI * (-1 / (360 / volleyAngle))) + (Mathf.PI * 0.5f));
        return transform.position +
               (new Vector3(Mathf.Cos(angle) * distanceFromCenter, 0, Mathf.Sin(angle) * distanceFromCenter));
    }
}