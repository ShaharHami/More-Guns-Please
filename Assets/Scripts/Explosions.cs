﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosions : MonoBehaviour
{
    public List<string> explosions;

    private void Start()
    {

    }

    public void Explode(string explosionName, Vector3 instantiationPosition, float delayBeforeDestroy)
    {
        if (explosions.Contains(explosionName))
        {
            GameObject explosionInstance =
                ObjectPooler.Instance.SpawnFromPool(explosionName, instantiationPosition, Quaternion.identity);
            StartCoroutine(TimedDisable(explosionInstance, delayBeforeDestroy));
        }
        else
        {
            print($"Did you misspell your explosion?! What is {explosionName}??");
        }
    }

    IEnumerator TimedDisable(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}