using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EventCallbacks
{
    public class Enemy : MonoBehaviour
    {
        public static List<Enemy> AllEnemies { get; private set; }
        [HideInInspector] public bool targetable = true;
        public GameObject shot;
        public float shotDestroyDelay = 10f;
        [Range(0.0f, 1.0f)] public float shotProbability;
        public float fireRateMin, fireRateMax;
        public bool lookAtPlayer = true;
        public float destroyDelay = 0f;
        private int enemyHealth = 50; //TODO: make this dynamic and centrally managed, off course
        private Explosions explosions;
        private GameObject shotInstance;
        private void OnEnable()
        {
            if (AllEnemies == null)
            {
                AllEnemies = new List<Enemy>();
            }
            AllEnemies.Add(this);
        }
        private void OnDisable()
        {
            AllEnemies.Remove(this);
        }
        void Start()
        {
            explosions = FindObjectOfType<Explosions>();
            MissleHitEvent.RegisterListener(OnMissleHit);
            if (shot != null)
            {
                float fireRate = Random.Range(fireRateMin, fireRateMax);
                InvokeRepeating("ShotLogic", fireRate, fireRate);
            }
        }
        void Update()
        {
            if (lookAtPlayer)
            {
                transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
            }
        }
        private void ShotLogic()
        {
            float range = Random.Range(0.0f, 1.0f);
            if (shotProbability >= range)
            {
                shotInstance = Instantiate(shot, transform.position, Quaternion.identity);
                Destroy(shotInstance, shotDestroyDelay);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Damage(enemyHealth);
            }
        }
        void OnDestroy()
        {
            MissleHitEvent.UnregisterListener(OnMissleHit);
        }

        void OnMissleHit(MissleHitEvent hit)
        {
            if (hit.UnitGO == gameObject)
            {
                targetable = true;
                Damage(hit.damage);
                // Debug.Log(hit.Description + hit.UnitGO.name + " And made " + hit.damage + " Damage.");
            }
        }
        void OnParticleCollision(GameObject other)
        {
            if (other.gameObject.CompareTag("Lasers"))
            {
                Damage(1); //TODO: get laser damage from central data manager
            }
        }
        void Damage(int damage)
        {
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                KillEnemy();
            }
        }
        void KillEnemy()
        {
            StopAllCoroutines();
            explosions.Explode("Enemy Death", transform.position, 2f);
            Destroy(gameObject, destroyDelay);
        }
    }
}
