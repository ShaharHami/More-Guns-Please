using UnityEngine;
using EventCallbacks;
using UnityEngine.Serialization;


public class EnemyShot : MonoBehaviour
{
    public GameObject hitEffect;
    public float shotSpeed = 100f;
    public bool isTargetingPlayer = false;
    public bool isHoming = false;
    [Range(0.1f, 1f)] public float homingDamp = 0.5f;
    public int damage = 10;
    public float life;
    public Vector3 origin;
    public Vector3 dest;
    private float killTimer;
    private Vector3 direction;
    private Explosions explosions;
    private Vector3 stageDimensions;
    private Vector3 targetPos;
    private Player player;
    public bool dirSet;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnEnable()
    {
        dirSet = false;
        direction = default;
        // if (player != null)
        // {
        //     if (isTargetingPlayer || isHoming)
        //     {
        //         GetTarget();
        //     }
        // }
        // else
        // {
        //     targetPos = new Vector3(transform.position.x, 0, transform.position.z - 500);
        // }
        killTimer = 0;
    }
    

    public void SetDir(Vector3 pos1, Vector3 pos2)
    {
        origin = pos1;
        dest = pos2;
        dirSet = true;
    }
    void FixedUpdate()
    {
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