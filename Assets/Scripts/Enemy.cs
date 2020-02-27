using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EventCallbacks
{
    public class Enemy : MonoBehaviour
    {
        public static List<Enemy> AllEnemies { get; private set; }
        public GameObject shot;
        [Range(0.0f, 1.0f)] public float shotProbability;
        public float fireRateMin, fireRateMax;
        public bool lookAtPlayer = true;
        [SerializeField] private float proximity = 2f;
        [SerializeField] private float flightSpeed = 0.01f;
        [SerializeField] private float destroyDelay = 1f;
        [SerializeField] private float slowTurnSpeed, fastTurnSpeed, turnDuration;
        [SerializeField] private int enemyHealth = 50; //TODO: make this dynamic and centrally managed, off course
        private int initialHealth;
        private Explosions explosions;
        private GameObject player;
        private Transform destinationPoint;
        private bool hasReachedPoint;
        public float TurnSpeed { get; set; }
        public enum EnemyState
        {
            Fly,
            ReachedFormation,
            InFormation,
            Dead
        };

        public EnemyState enemyState;

        private void OnEnable()
        {
            hasReachedPoint = false;
            enemyState = EnemyState.Fly;
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
            var position = transform.position;
            switch (enemyState)
            {
                case EnemyState.Fly:
                {
                    TurnSpeed = fastTurnSpeed;
                    LerpRotation(destinationPoint.transform.position - position);
                    break;
                }
                case EnemyState.ReachedFormation:
                {
                    TurnSpeed = slowTurnSpeed;
                    StartCoroutine(TurnSlowly());
                    LerpRotation(player.transform.position - position);
                    break;
                }
                case EnemyState.InFormation:
                {
                    LerpRotation(player.transform.position - position);
                    break;
                }
            }
        }

        IEnumerator TurnSlowly()
        {
            yield return new WaitForSeconds(turnDuration);
            TurnSpeed = fastTurnSpeed;
            enemyState = EnemyState.InFormation;
        }
        void LerpRotation(Vector3 dest)
        {
            Quaternion toRotation = Quaternion.LookRotation(dest);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, TurnSpeed * Time.time);
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
                Vector3 dest = Vector3.Lerp(transform.position, destinationPoint.transform.position, flightSpeed * Time.deltaTime);
                if (dir.sqrMagnitude <= proximity * proximity)
                {
                    enemyState = EnemyState.ReachedFormation;
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
            explosions.Explode("Enemy Death", transform.position, 2f);
            StartCoroutine(TimedDisable(gameObject, destroyDelay));
            
        }
        IEnumerator TimedDisable(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
            Transform thisParent = transform.parent;
            enemyHealth = initialHealth;
            EnemyDied enemyDied = new EnemyDied();
            enemyDied.Description = "Enemy " + gameObject.name + " has died ";
            enemyDied.enemy = transform;
            enemyDied.point = destinationPoint;
            enemyDied.parent = thisParent?.parent;
            enemyDied.FireEvent();
        }
    }
}
