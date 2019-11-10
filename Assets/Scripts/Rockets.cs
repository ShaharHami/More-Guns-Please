using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : MonoBehaviour
{
    [SerializeField] private Launcher[] rocketLaunchers;
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
        if (fireWeapon.fire)
        {
            foreach (Launcher launcher in rocketLaunchers)
            {
                launcher.Launch();
            }
        }
    }
}
