using System.Collections.Generic;
using System.Linq;
using EventCallbacks;
using UnityEngine;

[ExecuteInEditMode]
public class FiringManager : MonoBehaviour
{
    [SerializeReference] public List<ShootingPoints> shootingPointsList;

    private void OnEnable()
    {
        FireWeapon.RegisterListener(Shoot);
    }

    private void OnDisable()
    {
        FireWeapon.UnregisterListener(Shoot);
    }

    private void Shoot(FireWeapon info = null)
    {
        foreach (var shootingPoints in shootingPointsList.Where(shootingPoints => shootingPoints.enabled))
        {
            // shootingPoints.volley.shotPrefab.GetComponent<Shot>().shooter = info.shooter;
            shootingPoints.volley.SetShootingPoints(shootingPoints.GetShootingPoints());
            shootingPoints.volley.PerformSimpleVolley(info.fire);
        }
    }

    private void Awake()
    {
        if (shootingPointsList == null)
        {
            shootingPointsList = new List<ShootingPoints>();
        }
    }
}