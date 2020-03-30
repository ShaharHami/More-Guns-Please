using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class TargetingSystem : MonoBehaviour
{
    public Transform target;
    public Transform initialTarget;
    [SerializeField] private float speed = 100f;
    public bool targetingEnemy;
    public bool isHoming;
    private Player player;
    Vector3 direction;

    private void OnEnable()
    {
        Vector3 lookDirection = Vector3.forward;
        if (!targetingEnemy)
        {
            player = FindObjectOfType<Player>();
            lookDirection = transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        target = GetTarget();
        GetDirection();
    }

    private void FixedUpdate()
    {
        if (target && target.gameObject.activeSelf)
        {
            if (isHoming)
            {
                GetDirection();
            }
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

    private void GetDirection()
    {
        direction = target.position - transform.position;
        direction.y = 0;
        direction.Normalize();
    }

    private Transform GetTarget()
    {
        Transform target = initialTarget;
        if (targetingEnemy)
        {
            if (Enemy.AllEnemies == null) return target;
            List<Enemy> enemies = Enemy.AllEnemies.Where(enemy => gameObject.activeSelf).ToList();
            if (enemies.Count > 0)
            {
                target = enemies[Random.Range(0, enemies.Count)].transform;
            }
        }
        else if (player != null)
        {
            target = player.transform;
        }
        return target;
    }
}