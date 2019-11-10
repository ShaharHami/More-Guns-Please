using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private EventCallbacks.Rocket rocket;
    ObjectPooler pooler;
    private GameObject launchedRocket;
    private Transform closestEnemy;
    float distance;
    private void Start()
    {
        pooler = ObjectPooler.Instance;
    }
    public void Launch()
    {
        distance = 0;
        if (launchedRocket == null || !launchedRocket.activeSelf)
        {
            // launchedRocket = Instantiate(rocket.gameObject, transform.position, Quaternion.identity);
            launchedRocket = pooler.SpawnFromPool("Rocket", transform.position, Quaternion.identity);
            EventCallbacks.Rocket rocketComponent = launchedRocket.GetComponent<EventCallbacks.Rocket>();
            rocketComponent.launcher = this;
            // GetTarget(rocketComponent);
        }
    }

    public void GetTarget(EventCallbacks.Rocket rocket)
    {
        List<EventCallbacks.Enemy> enemies = EventCallbacks.Enemy.AllEnemies;
        Transform target;
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].targetable)
                {
                    float enemyDistance = (enemies[i].transform.position - transform.position).sqrMagnitude;
                    if (i == 0)
                    {
                        distance = enemyDistance;
                    }
                    if (enemyDistance <= distance)
                    {
                        distance = enemyDistance;
                        closestEnemy = enemies[i].transform;
                    }
                    if (i == enemies.Count - 1)
                    {
                        if (enemies.Count == 1)
                        {
                            closestEnemy = enemies[0].transform;
                            closestEnemy.GetComponent<EventCallbacks.Enemy>().targetable = true;
                        }
                        else if (closestEnemy == null || !closestEnemy.GetComponent<EventCallbacks.Enemy>().targetable)
                        {
                            closestEnemy = enemies[Random.Range(0, i)].transform;
                        }
                        else
                        {
                            closestEnemy.GetComponent<EventCallbacks.Enemy>().targetable = false;
                        }
                        target = closestEnemy;
                        rocket.target = target;
                    }
                }
            }
        }
    }
}
