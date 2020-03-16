using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
using JetBrains.Annotations;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject rocket;
    private GameObject launchedRocket;
    private Transform closestEnemy;
    float distance;
    public void Launch()
    {
        distance = 0;
        if (launchedRocket == null || !launchedRocket.activeSelf)
        {
            launchedRocket = ObjectPooler.Instance.SpawnFromPool(rocket.name, transform.position, Quaternion.identity);
            Rocket rocketComponent = launchedRocket.GetComponent<Rocket>();
            rocketComponent.launcher = this;
        }
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
}
