using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShootingLogic : MonoBehaviour
{
    public enum ShotType
    {
        Forward,
        Target,
        Homing
    };
    public ShotType shotType;
    [Range(0.0f, 1.0f)] public float shotProbability;
    public EnemyShot shot;
    public float shotSpeed = 100f;
    [Range(0.1f, 1f)] public float homingDamp = 0.5f;
    public Transform target;
    private Vector3 direction;
    private List<EnemyShot> enemyShots;
    // private Vector3 motion;
    private bool isHoming;
    private Vector3 enemyPosition;
    private Vector3 enemyForward;
    public Vector3 Motion;
    


    private void Awake()
    {
        if (target == null)
        {
            target = FindObjectOfType<Player>().transform;
        }
        enemyShots = new List<EnemyShot>();
    }

    public void Shoot()
    {
        if (target == null || !target.gameObject.activeInHierarchy) return;
        float range = Random.Range(0.0f, 1.0f);
        if (!(shotProbability >= range)) return;
        enemyPosition = transform.position;
        enemyForward = transform.forward;
        GameObject shotInstance =
            ObjectPooler.Instance.SpawnFromPool(shot.gameObject.name, enemyPosition, Quaternion.identity);
        EnemyShot enemyShot = shotInstance.GetComponent<EnemyShot>();
        enemyShots.Add(enemyShot);
    }

    private void FixedUpdate()
    {
        if (enemyShots.Count <= 0) return;
        // TODO: FIX THIS. If the target is not the player
        if (target == null)
        {
            target = FindObjectOfType<Player>().transform;
            if (target == null)
            {
                return;
            }
        }
        // fix end
        foreach (var enemyShot in enemyShots.Where(enemyShot => enemyShot.gameObject.activeSelf))
        {
            SetBehaviour(enemyShot);
            var motion = shotSpeed * GetDirection(enemyShot.dest, enemyShot.origin);
            if (shotType == ShotType.Homing)
            {
                motion = Vector3.Lerp(Vector3.zero, motion, homingDamp);
            }
            // print(motion);
            Motion = motion;
            enemyShot.transform.position += motion;
        }
    }

    private void SetBehaviour(EnemyShot enemyShot)
    {
        switch (shotType)
        {
            case ShotType.Forward:
                if (!enemyShot.dirSet)
                {
                    enemyShot.SetDir(enemyShot.transform.position, enemyForward); 
                }
                break;
            case ShotType.Target:
                if (!enemyShot.dirSet)
                {
                    enemyShot.SetDir(enemyShot.transform.position, target.position);
                }
                break;
            case ShotType.Homing:
                enemyShot.SetDir(enemyShot.transform.position, target.position);
                break;
        }
        // print(enemyShot.dir);
    }

    private Vector3 GetDirection(Vector3 targetVector, Vector3 enemyShotTransform)
    {
        var getDirection = targetVector - enemyShotTransform;
        getDirection.y = 0;
        getDirection.Normalize();
        return getDirection;
    }
}