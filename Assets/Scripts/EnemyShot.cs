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
        private Vector3 direction;
        private Transform player;
        private Explosions explosions;
        private void Start()
        {
            GetTarget();
            explosions = FindObjectOfType<Explosions>();
        }

        private void GetTarget()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
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
            Vector3 motion = direction * shotSpeed * Time.deltaTime;
            motion.y = 0;
            transform.position += motion;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Destroy(gameObject);
                EnemyShotHit enemyShotHit = new EnemyShotHit();
                enemyShotHit.Description = "Player " + gameObject.name + " has hit ";
                enemyShotHit.UnitGO = gameObject;
                enemyShotHit.damage = damage;
                enemyShotHit.FireEvent();
                explosions.Explode("Missile Hit", transform.position, 1f);
            }
        }
    }
}