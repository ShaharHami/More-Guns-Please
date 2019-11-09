using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private EventCallbacks.Rocket rocket;
    private GameObject launchedRocket;
    private Transform closestEnemy;
    float distance;
    public void Launch()
    {
        distance = 0;
        if (launchedRocket == null)
        {
            launchedRocket = Instantiate(rocket.gameObject, transform.position, Quaternion.identity);
            EventCallbacks.Rocket rocketComponent = launchedRocket.GetComponent<EventCallbacks.Rocket>();
            rocketComponent.launcher = this;
            // GetTarget(rocketComponent);
        }
    }

    public void GetTarget(EventCallbacks.Rocket rocket)
    {
        EventCallbacks.Enemy[] enemies = FindObjectsOfType<EventCallbacks.Enemy>();
        Transform target;
        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
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
                    if (i == enemies.Length - 1)
                    {
                        if (enemies.Length == 1)
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
