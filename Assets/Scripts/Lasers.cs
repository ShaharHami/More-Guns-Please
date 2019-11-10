using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] shots;
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
            foreach (ParticleSystem shot in shots)
            {
                shot.Play();
            }
        }
        else
        {
            foreach (ParticleSystem shot in shots)
            {
                shot.Stop();
            }
        }
    }
}
