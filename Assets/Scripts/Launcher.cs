using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Rocket rocket;
    [SerializeField] private float speed = 100f;
    private GameObject launchedRocket;
    private Transform closestEnemy;
    float distance;
    public void Launch()
    {
        distance = 0;
        if (launchedRocket == null)
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            if (enemies[0] != null)
            {
                // closestEnemy = enemies[0].transform;
                for (int i = 0; i < enemies.Length; i++)
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
                    if (i == enemies.Length-1)
                    {
                        launchedRocket = Instantiate(rocket.gameObject, transform.position, Quaternion.identity);
                        launchedRocket.GetComponent<Rocket>().target = closestEnemy;
                    }
                }
                Destroy(launchedRocket, 2f);
            }
        }
    }
}
