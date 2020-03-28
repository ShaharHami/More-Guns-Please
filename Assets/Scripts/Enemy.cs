using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
using Random = UnityEngine.Random;


public class Enemy : MonoBehaviour
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
    private Explosions explosions;
    private GameObject player;
    private Transform destinationPoint;
    private bool hasReachedPoint;
    private Collider[] hitColliders;
    private Vector3 cummulativeDir;
    private ShootingLogic shootingLogic;
    Vector3 dest;

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
    }

    private void OnEnable()
    {
        healthDisplay.SetHealth(enemyHealth, false);
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
        MissleHitEvent.RegisterListener(OnMissleHit);
        shootingLogic = GetComponent<ShootingLogic>();
        if (shootingLogic != null)
        {
            float fireRate = Random.Range(fireRateMin, fireRateMax);
            InvokeRepeating(nameof(ShotLogic), fireRateMin, fireRate);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ShotLogic));
        AllEnemies.Remove(this);
        AllEnemieGOs.Remove(gameObject);
        MissleHitEvent.UnregisterListener(OnMissleHit);
    }

    void Start()
    {
        explosions = FindObjectOfType<Explosions>();
        initialHealth = enemyHealth;
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
                enemyState = EnemyState.SlowTurning;
                break;
            }
            case EnemyState.SlowTurning:
            {
                TurnSpeed = slowTurnSpeed;

                LerpRotation(Destination() - position);
                break;
            }
            case EnemyState.InFormation:
            {
                TurnSpeed = fastTurnSpeed;
                LerpRotation(Destination() - position);
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
        if (!player.activeInHierarchy)
        { 
            enemyState = EnemyState.SlowTurning;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, TurnSpeed * Time.fixedDeltaTime);
    }

    private void ShotLogic()
    {
        // if (player != null && player.gameObject.activeInHierarchy)
        // {
        //     float range = Random.Range(0.0f, 1.0f);
        //     if (shotProbability >= range)
        //     {
        //         GameObject shotInstance = ObjectPooler.Instance.SpawnFromPool(shot.name, transform.position, Quaternion.identity);
        //         // shotInstance.transform.parent = transform;
        //         EnemyShot enemyShot = shotInstance.GetComponent<EnemyShot>();
        //         enemyShot.SetDir(transform.forward);
        //     }
        // }
        shootingLogic.Shoot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Damage(enemyHealth);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        print(other.collider.name);
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
            Damage(2); //TODO: get laser damage from central data manager
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
        healthDisplay.ChangeHealth(enemyHealth);
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
        enemyHealth = initialHealth;
        EnemyDied enemyDied = new EnemyDied();
        enemyDied.Description = "Enemy " + gameObject.name + " has died ";
        enemyDied.enemy = transform;
        enemyDied.point = destinationPoint;
        enemyDied.parent = thisParent?.parent;
        enemyDied.FireEvent();
    }
}