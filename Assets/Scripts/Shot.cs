using System;
using EventCallbacks;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public string tagToCompare;
    public string explosionName;
    public float life;
    public bool scatterShot;
    public Volley volley;
    private Explosions explosions;
    private float killTimer;

    void Start()
    {
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnEnable()
    {
        killTimer = 0;
        volley = GetComponent<Volley>();
    }

    private void OnDisable()
    {
        killTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCompare)) return;
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
            MissleHitEvent missileHit = new MissleHitEvent();
            missileHit.Description = "Unit " + gameObject.name + " has hit ";
            missileHit.damage = 3; //TODO: get rocket damage from central data manager
            missileHit.UnitGO = target.gameObject;
            missileHit.FireEvent();
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