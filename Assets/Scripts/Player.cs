using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EventCallbacks;
using TMPro;
using UnityEditor.SceneManagement;

public class Player : MonoBehaviour
{
    public int playerHealth = 100;
    public Vector3 spawnPoint;
    private int storePlayerHealth;
    private DoABarrelRoll doABarrelRoll;
    private HealthDisplay healthDisplay;
    private Explosions explosions;
    [HideInInspector] public bool lastFrameShooting;
    private FireInputManager fireInputManager;

    public string activeShot { get; private set; }

    // Debugging
    public TextMeshProUGUI hitsDisplay;
    

    private void Awake()
    {
        fireInputManager = GetComponent<FireInputManager>();
        explosions = FindObjectOfType<Explosions>();
        healthDisplay = GetComponent<HealthDisplay>();
        lastFrameShooting = !fireInputManager.FireInput();
        storePlayerHealth = playerHealth;
    }

    private void OnEnable()
    {
        doABarrelRoll = GetComponent<DoABarrelRoll>();
        EnemyShotHit.RegisterListener(OnDamage);
        healthDisplay.SetHealth(storePlayerHealth, false);
    }

    private void Update()
    {
        Shoot(fireInputManager.FireInput() && !doABarrelRoll.isRotating);
        if (doABarrelRoll.isRotating)
        {
            lastFrameShooting = !fireInputManager.FireInput();
        }
        else
        {
            lastFrameShooting = fireInputManager.FireInput();
        }
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
    }

    void OnDisable()
    {
        EnemyShotHit.UnregisterListener(OnDamage);
    }

    private void Shoot(bool fire)
    {
        if (lastFrameShooting == fireInputManager.FireInput()) return;
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