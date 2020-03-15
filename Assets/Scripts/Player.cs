using System;
using System.Collections;
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
    public Vector3 spawnPoint;
    [HideInInspector] public bool shooting;
    private int storePlayerHealth;
    private DoABarrelRoll doABarrelRoll;
    private HealthDisplay healthDisplay;
    private Explosions explosions;

    public string activeShot { get; private set; }

    // Debugging
    public TextMeshProUGUI hitsDisplay;

    private void Awake()
    {
        explosions = FindObjectOfType<Explosions>();
        healthDisplay = GetComponent<HealthDisplay>();
        storePlayerHealth = playerHealth;
    }

    private void OnEnable()
    {
        doABarrelRoll = GetComponent<DoABarrelRoll>();
        EnemyShotHit.RegisterListener(OnDamage);
        healthDisplay.SetHealth(storePlayerHealth, false);
        activeShot = shots[0].type;
        SetInitialShotsLevel();
    }

    private void SetInitialShotsLevel()
    {
        foreach (Shot shot in shots)
        {
            if (shot.type != activeShot)
            {
                shot.SetInactive();
            }
            else
            {
                shot.SetLevel(1);
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

    private void Damage(int damage)
    {
        playerHealth -= damage;
        healthDisplay.ChangeHealth(playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        explosions.Explode("Player Death", transform.position, 2f);
        PlayerDied playerDied = new PlayerDied();
        playerDied.Description = "Player Died";
        playerDied.respawnTimer = 5f;
        playerDied.FireEvent();
        gameObject.SetActive(false);
        playerHealth = storePlayerHealth;
        transform.position = spawnPoint;
        SetInitialShotsLevel();
    }

    void OnDisable()
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

    // BLAHHHHHYHHHHH
    // Debugging
    Dictionary<string, int> hitsTaken = new Dictionary<string, int>();
    int counter;
    string message;

    private void OnDamage(EnemyShotHit hit)
    {
        Damage(hit.damage);
        // Debugging
        if (hitsTaken.Keys.Contains(hit.UnitGO.name))
        {
            hitsTaken[hit.UnitGO.name] += hit.damage;
        }
        else
        {
            hitsTaken.Add(hit.UnitGO.name, hit.damage);
        }

        RenderMessage();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            int amount = enemy.initialHealth;
            // Debugging
            if (hitsTaken.Keys.Contains(other.gameObject.name))
            {
                hitsTaken[other.gameObject.name] += amount;
            }
            else
            {
                hitsTaken.Add(other.gameObject.name, amount);
            }

            Damage(amount);
        }

        // Debugging
        RenderMessage();
    }

    private void RenderMessage()
    {
        if (hitsDisplay == null) return;
        message = "";
        foreach (var hitType in hitsTaken.Keys)
        {
            message += "Total Damage From: " + hitType + ": " + hitsTaken[hitType] + Environment.NewLine;
        }
        hitsDisplay.text = message;
    }
}