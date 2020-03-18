using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject rocket;
    public float rocketInterval;
    private GameObject launchedRocket;
    private Transform closestEnemy;
    private Coroutine shootingCoroutine;
    private bool fire;
    private float timer;
    private bool startTimer;
    private bool isRunning;

    private void OnEnable()
    {
        FireWeapon.RegisterListener(Shoot);
    }

    private void OnDisable()
    {
        FireWeapon.UnregisterListener(Shoot);
        StopTheCoroutine();
    }

    private void Update()
    {
        if (!startTimer) return;
        HandleTimer();
    }

    private void HandleTimer()
    {
        timer += Time.deltaTime;
        if (!(timer >= rocketInterval)) return;
        StopTheCoroutine();
        startTimer = false;
        timer = 0;
    }

    private void Launch()
    {
        launchedRocket = ObjectPooler.Instance.SpawnFromPool(rocket.name, transform.position, Quaternion.identity);
        Rocket rocketComponent = launchedRocket.GetComponent<Rocket>();
        rocketComponent.launcher = this;
    }

    public Transform GetTarget(Rocket obj)
    {
        List<Enemy> enemies = new List<Enemy>();
        Transform target;
        if (Enemy.AllEnemies == null) return obj.initialTarget;
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count > 0)
        {
            target = enemies[Random.Range(0, enemies.Count)].transform;
        }
        else
        {
            target = obj.initialTarget;
        }

        return target;
    }

    private IEnumerator ShootingLogic()
    {
        while (gameObject.activeSelf)
        {
            if (fire)
            {
                Launch();
            }

            yield return new WaitForSeconds(rocketInterval);
        }
    }

    private void Shoot(FireWeapon fireWeapon)
    {
        fire = fireWeapon.fire;
        startTimer = !fire;
        if (shootingCoroutine == null || !isRunning)
        {
            StartTheCoroutine();
        }
    }

    private void StartTheCoroutine()
    {
        shootingCoroutine = StartCoroutine(ShootingLogic());
        isRunning = true;
    }

    private void StopTheCoroutine()
    {
        StopCoroutine(shootingCoroutine);
        isRunning = false;
    }
}