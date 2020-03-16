using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : MonoBehaviour
{
    [SerializeField] private Launcher[] rocketLaunchers;
    [SerializeField] private float rocketInterval;
    private Coroutine shootingCoroutine;
    private void Start()
    {
        EventCallbacks.FireWeapon.RegisterListener(Shoot);
    }
    private void OnDestroy()
    {
        EventCallbacks.FireWeapon.UnregisterListener(Shoot);
    }
    private void Shoot(EventCallbacks.FireWeapon fireWeapon)
    {
        if (!fireWeapon.fire && shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
        shootingCoroutine = StartCoroutine(ShootingLogic(fireWeapon.fire));
    }

    private IEnumerator ShootingLogic(bool fire)
    {
        while (fire && gameObject.activeInHierarchy)
        {
            foreach (Launcher launcher in rocketLaunchers)
            {
                launcher.Launch();
            }
            yield return new WaitForSeconds(rocketInterval);
        }
    }
}
