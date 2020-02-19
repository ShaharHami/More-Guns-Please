using System.Collections;
using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] shots;
    private void OnEnable()
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
            foreach (ParticleSystem shot in shots)
            {
                ParticleSystem.EmissionModule e = shot.emission;
                e.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem shot in shots)
            {
                ParticleSystem.EmissionModule e = shot.emission;
                e.enabled = false;
            }
        }
    }
}
