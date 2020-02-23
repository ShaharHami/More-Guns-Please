using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private Transform destinationPoint;

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

        public void FlyToPoint(Transform point)
        {
            if (!gameObject.activeInHierarchy || !gameObject.activeSelf) return;
            destinationPoint = point;
            StartCoroutine(FlyToPointCoroutine());
        }
        IEnumerator FlyToPointCoroutine()
        {
            while (true)
            {
                if (!gameObject.activeInHierarchy || !gameObject.activeSelf) yield break;
                Vector3 dir = transform.position - destinationPoint.position;
                Vector3 dest = Vector3.Lerp(transform.position, destinationPoint.transform.position, 0.05f);
                if (dir.sqrMagnitude <= 1f * 1f)
                {
                    ReachedPoint reachedPoint = new ReachedPoint();
                    reachedPoint.Description = "Enemy " + gameObject.name + " has reached destination";
                    reachedPoint.objTransform = transform;
                    reachedPoint.parentTransform = destinationPoint;
                    reachedPoint.FireEvent();
                    yield break;
                }
                transform.position = dest;
                yield return null;
            }
        }
        void KillEnemy()
        {
            Transform thisParent = transform.parent;
            enemyHealth = initialHealth;
            explosions.Explode("Enemy Death", transform.position, 2f);
            EnemyDied enemyDied = new EnemyDied();
            enemyDied.Description = "Enemy " + gameObject.name + " has died ";
            enemyDied.enemy = transform;
            enemyDied.point = destinationPoint;
            if (thisParent != null)
            {
                enemyDied.parent = thisParent.parent;
            }
            enemyDied.FireEvent();
            gameObject.SetActive(false);
        }
    }
}
