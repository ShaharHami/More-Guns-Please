using System;
using System.Collections;
using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public float speed;
    public string moduleName;
    public string propName;
    public float upgradeValue;
    
    private void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speed * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        UpgradeEvent upgradeEvent = new UpgradeEvent
        {
            moduleName = moduleName, propName = propName, upgradeValue = upgradeValue
        };
        upgradeEvent.FireEvent();
        gameObject.SetActive(false);
    }
}
