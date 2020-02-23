using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public class EnemyShot : MonoBehaviour
    {
        public float shotSpeed = 100f;
        public bool isHoming = false;
        public int damage = 10;
        public float life;
        float killTimer;
        private Vector3 direction;
        private Transform player;
        private Explosions explosions;
        private Vector3 stageDimensions;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            explosions = FindObjectOfType<Explosions>();
        }

        private void OnEnable()
        {
            GetTarget();
            killTimer = 0;
        }

        private void GetTarget()
        {
            if (player == null) return;
            direction = player.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
        }

        void FixedUpdate()
        {
            if (isHoming)
            {
                GetTarget();
            }
            Vector3 motion = Time.deltaTime * shotSpeed * direction;
            motion.y = 0;
            transform.position += motion;
            killTimer += Time.fixedDeltaTime;
            if (killTimer >= life)
            {
                gameObject.SetActive(false);
                killTimer = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                EnemyShotHit enemyShotHit = new EnemyShotHit();
                enemyShotHit.Description = "Player " + gameObject.name + " has hit ";
                enemyShotHit.UnitGO = gameObject;
                enemyShotHit.damage = damage;
                enemyShotHit.FireEvent();
                explosions.Explode("Enemy Shot Hit", transform.position, 1f);
            }
        }
    }
}