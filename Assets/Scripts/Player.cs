using System;
using UnityEngine;
using EventCallbacks;
using ForceShield;

public class Player : MonoBehaviour
{
    public Vector3 spawnPoint;
    public DoABarrelRoll doABarrelRoll;
    private Explosions explosions;
    [HideInInspector] public bool lastFrameShooting;
    private FireInputManager fireInputManager;
    public ForceShieldController forceShieldController;

    private void Awake()
    {
        fireInputManager = GetComponent<FireInputManager>();
        explosions = FindObjectOfType<Explosions>();
        lastFrameShooting = !fireInputManager.FireInput();
    }

    private void OnEnable()
    {
        // Invoke(nameof(StartForceShield), 0.5f);
    }

    private void StartForceShield()
    {
        forceShieldController.EnableShield();
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

    public void KillPlayer()
    {
        explosions.Explode("Player Death", transform.position, 2f);
        PlayerDied playerDied = new PlayerDied {Description = "Player Died", respawnTimer = 5f};
        playerDied.FireEvent();
        gameObject.SetActive(false);
        transform.position = spawnPoint;
    }

    private void Shoot(bool fire)
    {
        if (lastFrameShooting == fireInputManager.FireInput()) return;
        FireWeapon fireWeaponEvent = new FireWeapon
            {Description = "Fire weapon: " + fire, fire = fire, shooter = gameObject};
        fireWeaponEvent.FireEvent();
    }
}