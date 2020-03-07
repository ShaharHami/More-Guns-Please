using UnityEngine;
using EventCallbacks;


public class EnemyShot : MonoBehaviour
{
    public GameObject hitEffect;
    public float shotSpeed = 100f;
    public bool isTargetingPlayer = false;
    public bool isHoming = false;
    [Range(0.1f, 1f)] public float homingDamp = 0.5f;
    public int damage = 10;
    public float life;
    float killTimer;
    private Vector3 direction;
    private Transform player;
    private Explosions explosions;
    private Vector3 stageDimensions;
    private Vector3 targetPos;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnEnable()
    {
        if (player != null && isTargetingPlayer)
        {
            targetPos = player.position;
        }
        else
        {
            targetPos = new Vector3(transform.position.x, 0, transform.position.z - 500);
        }

        GetTarget();
        killTimer = 0;
    }

    private void GetTarget()
    {
        direction = targetPos - transform.position;
        direction.y = 0;
        direction.Normalize();
    }

    void FixedUpdate()
    {
        if (player != null && isHoming)
        {
            targetPos = player.position;
            GetTarget();
        }

        Vector3 motion = Time.deltaTime * shotSpeed * direction;
        motion.y = 0;
        if (isHoming)
        {
            motion = Vector3.Lerp(Vector3.zero, motion, 0.5f);
        }

        transform.position += motion;
        killTimer += Time.fixedDeltaTime;
        if (killTimer >= life)
        {
            gameObject.SetActive(false);
            explosions.Explode(hitEffect.name, transform.position, 1f);
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
            explosions.Explode(hitEffect.name, transform.position, 1f);
        }
    }
}