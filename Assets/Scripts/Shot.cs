using System;
using System.Linq;
using EventCallbacks;
using UnityEngine;

public class Shot : MonoBehaviour, ICauseDamage
{
    public string[] tagsToCompare;
    public string explosionName;
    public float life;
    public bool scatterShot;
    public Volley volley;
    public GameObject shooter;
    public int DamageAmount;
    public int damageAmount { get; set; }
    private Explosions explosions;
    private float killTimer;

    void Start()
    {
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnEnable()
    {
        damageAmount = DamageAmount;
        killTimer = 0;
        if (volley == null)
        {
            volley = GetComponent<Volley>();
        }
    }

    private void OnDisable()
    {
        killTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!tagsToCompare.Contains(other.gameObject.tag)) return;
        killTimer = 0;
        Explode(other.gameObject);
    }

    private void FixedUpdate()
    {
        killTimer += Time.fixedDeltaTime;
        if ((killTimer < life)) return;
        killTimer = 0;
        Explode(null);
    }

    private void Explode(GameObject target)
    {
        if (!gameObject.activeSelf) return;
        if (target != null)
        {
            ProjectileHitEvent projectileHitEvent = new ProjectileHitEvent();
            projectileHitEvent.Description = "Unit " + gameObject.name + " has hit ";
            projectileHitEvent.damage = 3; //TODO: get rocket damage from central data manager
            projectileHitEvent.shooter = shooter;
            projectileHitEvent.target = target;
            projectileHitEvent.projectile = transform;
            projectileHitEvent.FireEvent();
        }

        if (scatterShot && volley != null)
        {
            volley.PerformSimpleVolley(true);
        }

        if (explosionName != "")
        {
            explosions.Explode(explosionName, transform.position, 1f);
        }

        gameObject.SetActive(false);
    }
}