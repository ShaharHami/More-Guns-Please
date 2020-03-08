using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EventCallbacks;
using TMPro;

public class Player : MonoBehaviour
{
    public int playerHealth = 100;
    public Shot[] shots;
    public bool autoFire;
    [Range(0.0f, 0.5f)] public float marginLeft = 0f, marginRight = 0f, marginTop = 0f, marginBottom = 0f;
    private bool shooting;
    private DoABarrelRoll doABarrelRoll;
    public string activeShot { get; private set; }

    public TextMeshProUGUI hitsDisplay;

    void Start()
    {
        doABarrelRoll = GetComponent<DoABarrelRoll>();
        EnemyShotHit.RegisterListener(OnDamage);
        activeShot = shots[0].type;
        foreach (Shot shot in shots)
        {
            if (shot.type != activeShot)
            {
                shot.SetInactive();
            }
            else
            {
                shot.SetLevel(0);
            }
        }
    }

    void Update()
    {
        if (autoFire)
        {
            shooting = true;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    shooting = true;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    shooting = false;
                }
            }

            if (Input.GetMouseButton(0)) // Mouse support for debugging purposes
            {
                shooting = true;
            }
            else
            {
                shooting = false;
            }

            if (Input.GetKey(KeyCode.Space)) // keyboard support for debugging purposes
            {
                shooting = true;
            }
        }
        
        if (shooting)
        {
            Shoot(!doABarrelRoll.isRotating);
        }
        else
        {
            Shoot(false);
        }
    }

    public void SetAutoFire(bool setAutoFire)
    {
        autoFire = setAutoFire;
    }
    // BLAHHHHHYHHHHH
    Dictionary<string, int> hitsTaken = new Dictionary<string, int>();
    int counter;
    string message;
    private void OnDamage(EnemyShotHit hit)
    {
        if (doABarrelRoll.isRotating)
        {
            print($"woohoo got away from {hit.UnitGO.name}!");
            return;
        }
        Damage(hit.damage);
        if (hitsTaken.Keys.Contains(hit.UnitGO.name))
        {
            hitsTaken[hit.UnitGO.name] += 1;
        }
        else
        {
            hitsTaken.Add(hit.UnitGO.name, 1);
        }

        RenderMessage();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (doABarrelRoll.isRotating) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (hitsTaken.Keys.Contains(other.gameObject.name))
            {
                hitsTaken[other.gameObject.name] += 1;
            }
            else
            {
                hitsTaken.Add(other.gameObject.name, 1);
            }
            Damage(20);
        }

        RenderMessage();
    }

    private void RenderMessage()
    {
        message = "";
        foreach (var hitType in hitsTaken.Keys)
        {
            message += hitType + " | " + hitsTaken[hitType] + Environment.NewLine;
        }
        if (hitsDisplay != null)
        {
            hitsDisplay.text = message;
        }
    }
    private void Damage(int damage)
    {
        playerHealth -= damage;
    }

    void OnDestroy()
    {
        EnemyShotHit.UnregisterListener(OnDamage);
    }



    private void Shoot(bool fire)
    {
        FireWeapon fireWeaponEvent = new FireWeapon();
        fireWeaponEvent.Description = "Fire weapon: " + fire;
        fireWeaponEvent.fire = fire;
        fireWeaponEvent.FireEvent();
    }
}