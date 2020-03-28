using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class Homing : MonoBehaviour
{
    public Transform target;
    public Transform initialTarget;
    [SerializeField] private float speed = 100f;
    Vector3 direction;

    private void OnEnable()
    {
        target = GetTarget();
    }

    private void FixedUpdate()
    {
        if (target && target.gameObject.activeSelf)
        {
            direction = target.position - transform.position;
            direction.y = 0;
            direction.Normalize();
        }
        else
        {
            target = initialTarget;
        }
        Vector3 motion = Time.fixedDeltaTime * speed * direction;
        motion.y = 0;
        transform.position += motion;
        transform.LookAt(target);
    }

    private Transform GetTarget()
    {
        Transform target = initialTarget;
        if (Enemy.AllEnemies == null) return target;
        List<Enemy> enemies = Enemy.AllEnemies.Where(enemy => gameObject.activeSelf).ToList();
        if (enemies.Count > 0)
        {
            target = enemies[Random.Range(0, enemies.Count)].transform;
        }

        return target;
    }
}