using System;
using System.Collections;
using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    public GameObject[] shots;
    private List<ParticleSystem> particleSystems;

    private void OnEnable()
    {
        FireWeapon.RegisterListener(Shoot);
        RegisterParticleSystems();
    }

    private void OnDestroy()
    {
        FireWeapon.UnregisterListener(Shoot);
    }

    void RegisterParticleSystems()
    {
        particleSystems = new List<ParticleSystem>();
        foreach (var shot in shots)
        {
            if (shot == null) return;
            particleSystems.Add(shot.GetComponent<ParticleSystem>());
        }
    }

    private void Shoot(FireWeapon fireWeapon)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if (particleSystem == null)
            {
                RegisterParticleSystems();
                return;
            }
            
            ParticleSystem.EmissionModule e = particleSystem.emission;
            e.enabled = fireWeapon.fire;
        }
    }
}