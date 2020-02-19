using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EventCallbacks
{
    public class Enemy : MonoBehaviour
    {
        public static List<Enemy> AllEnemies { get; private set; }
        public GameObject shot;
        [Range(0.0f, 1.0f)] public float shotProbability;
        public float fireRateMin, fireRateMax;
        public bool lookAtPlayer = true;
        [SerializeField] private int enemyHealth = 50; //TODO: make this dynamic and centrally managed, off course
        private int initialHealth;
        private Explosions explosions;
        private GameObject player;

        public enum EnemyState
        {
            Fly,
            Formation,
            Dead
        };

        private EnemyState enemyState;
        private void OnEnable()
        {
            if (AllEnemies == null)
            {
                AllEnemies = new List<Enemy>();
            }
            AllEnemies.Add(this);
            player = GameObject.FindGameObjectWithTag("Player");
            MissleHitEvent.RegisterListener(OnMissleHit);
            if (shot != null)
            {
                float fireRate = Random.Range(fireRateMin, fireRateMax);
                InvokeRepeating(nameof(ShotLogic), fireRate, fireRate);
            }
        }
        private void OnDisable()
        {
            CancelInvoke(nameof(ShotLogic));
            AllEnemies.Remove(this);
            MissleHitEvent.UnregisterListener(OnMissleHit);
        }
        void Start()
        {
            explosions = FindObjectOfType<Explosions>();
            initialHealth = enemyHealth;
        }
        void FixedUpdate()
        {
            if (player == null || !lookAtPlayer) return;
            transform.LookAt(player.transform);
        }
        private void ShotLogic()
        {
            float range = Random.Range(0.0f, 1.0f);
            if (shotProbability >= range)
            {
                ObjectPooler.Instance.SpawnFromPool("Enemy Shot", transform.position, Quaternion.identity);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
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
                Damage(hit.damage);
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
            gameObject.SetActive(false);
            enemyHealth = initialHealth;
            explosions.Explode("Enemy Death", transform.position, 2f);
            EnemyDied enemyDied = new EnemyDied();
            enemyDied.Description = "Enemy " + gameObject.name + " has died ";
            enemyDied.parent = transform.parent;
            enemyDied.FireEvent();
        }
    }
}
