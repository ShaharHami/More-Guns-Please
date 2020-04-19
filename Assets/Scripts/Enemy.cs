using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
using Random = UnityEngine.Random;


public class Enemy : MonoBehaviour, IDamagable, ICauseDamage
{
    public static List<Enemy> AllEnemies { get; private set; }
    public static List<GameObject> AllEnemieGOs { get; private set; }
    public GameObject shot;
    [Range(0.0f, 1.0f)] public float shotProbability;
    public float fireRateMin, fireRateMax;
    public bool lookAtPlayer = true;
    [SerializeField] private float proximity = 2f;
    [SerializeField] private float flightSpeed = 0.01f;
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private float slowTurnSpeed, fastTurnSpeed, turnDuration;
    [SerializeField] private int enemyHealth = 50; //TODO: make this dynamic and centrally managed, off course
    [SerializeField] private float avoidenceRadius;
    [SerializeField] private Color gizmoColor;
    public int initialHealth;
    public int health { get; set; }
    public int Health;
    public int maxHealth { get; set; }
    public int damageAmount { get; set; }
    public int DamageAmount;
    private Explosions explosions;
    private GameObject player;
    private Transform destinationPoint;
    private bool hasReachedPoint;
    private Collider[] hitColliders;
    private Vector3 cummulativeDir;
    private ShootingLogic shootingLogic;
    Vector3 dest;
    public Volley volley;

    private HealthDisplay healthDisplay;
    public float TurnSpeed { get; set; }

    public enum EnemyState
    {
        Fly,
        ReachedFormation,
        SlowTurning,
        InFormation,
        Dead
    };

    public EnemyState enemyState;

    private void Awake()
    {
        healthDisplay = GetComponent<HealthDisplay>();
        if (volley == null)
        {
            volley = GetComponent<Volley>();
        }
    }

    private void OnEnable()
    {
        maxHealth = health = Health;
        damageAmount = DamageAmount;
        healthDisplay.SetHealth(health, false);
        cummulativeDir = new Vector3();
        hasReachedPoint = false;
        enemyState = EnemyState.Fly;
        if (AllEnemies == null)
        {
            AllEnemies = new List<Enemy>();
            AllEnemieGOs = new List<GameObject>();
        }

        AllEnemies.Add(this);
        AllEnemieGOs.Add(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
        // ProjectileHitEvent.RegisterListener(OnMissleHit);
        float fireRate = Random.Range(fireRateMin, fireRateMax);
        InvokeRepeating(nameof(ShotLogic), fireRateMin, fireRate);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ShotLogic));
        AllEnemies.Remove(this);
        AllEnemieGOs.Remove(gameObject);
        // ProjectileHitEvent.UnregisterListener(OnMissleHit);
    }

    void Start()
    {
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, avoidenceRadius);
//        Gizmos.DrawLine(transform.position, cummulativeDir);
        Gizmos.DrawRay(transform.position, cummulativeDir);
    }

    private Vector3 AvoidNeigbours()
    {
        hitColliders = Physics.OverlapSphere(transform.position, avoidenceRadius); //TODO Layer mask
        if (hitColliders.Length <= 0)
        {
            return Vector3.zero;
        }

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != gameObject && AllEnemieGOs.Contains(collider.gameObject))
            {
                cummulativeDir += (transform.position - collider.transform.position);
            }
        }

        cummulativeDir /= hitColliders.Length;
        return cummulativeDir;
    }

    private void LateUpdate()
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
                enemyState = EnemyState.SlowTurning;
                break;
            }
            case EnemyState.SlowTurning:
            {
                TurnSpeed = slowTurnSpeed;

                LerpRotation(Destination() - position);
                // enemyState = EnemyState.InFormation;
                break;
            }
            case EnemyState.InFormation:
            {
                TurnSpeed = fastTurnSpeed;
                LerpRotation(Destination() - position);
                // transform.LookAt(Destination());
                break;
            }
        }
    }

    private Vector3 Destination()
    {
        if (player == null)
        {
            return dest;
        }

        if (!player.activeSelf)
        {
            enemyState = EnemyState.ReachedFormation;
        }
        else
        {
            if (lookAtPlayer)
            {
                dest = player.transform.position;
            }
            else
            {
                dest = transform.position - Vector3.forward;
            }
        }

        return dest;
    }

    IEnumerator TurnSlowly()
    {
        yield return new WaitForSeconds(turnDuration);
        enemyState = EnemyState.InFormation;
    }

    void LerpRotation(Vector3 dest)
    {
        Quaternion toRotation = Quaternion.LookRotation(dest);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, TurnSpeed * Time.deltaTime);
    }

    private void ShotLogic()
    {
        if (player == null || !player.activeSelf) return;
        // volley.shotPrefab.GetComponent<Shot>().shooter = gameObject;
        var rnd = Random.Range(0f, 1f) < shotProbability;
        volley.PerformSimpleVolley(rnd);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     Damage(enemyHealth);
        // }
    }

    // void OnMissleHit(ProjectileHitEvent hit)
    // {
    //     // if (hit.shooter == gameObject)
    //     // {
    //     //     Damage(hit.damage);
    //     // }
    // }

    // void OnParticleCollision(GameObject other)
    // {
    //     if (other.gameObject.CompareTag("Lasers"))
    //     {
    //         Damage(2); //TODO: get laser damage from central data manager
    //     }
    // }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            KillEnemy();
        }

        healthDisplay.ChangeHealth(health);
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

            Vector3 dest = Vector3.Lerp(transform.position, destinationPoint.position,
                flightSpeed * Time.deltaTime);
            transform.position = dest;
            yield return null;
        }
    }

    public void KillEnemy(bool silent = false)
    {
        StartCoroutine(TimedDisable(gameObject, destroyDelay));
        if (silent) return;
        explosions.Explode("Enemy Death", transform.position, 2f);
    }

    IEnumerator TimedDisable(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        Transform thisParent = transform.parent;
        health = maxHealth;
        EnemyDied enemyDied = new EnemyDied();
        enemyDied.Description = "Enemy " + gameObject.name + " has died ";
        enemyDied.enemy = transform;
        enemyDied.point = destinationPoint;
        enemyDied.parent = thisParent?.parent;
        enemyDied.FireEvent();
    }
}