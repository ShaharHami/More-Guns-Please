using System;
using EventCallbacks;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public string tagToCompare;
    public string explosionName;
    public float life;
    private Explosions explosions;
    private float killTimer;

    void Start()
    {
        explosions = FindObjectOfType<Explosions>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagToCompare))
        {
            Explode(other.gameObject);
            killTimer = 0;
        }
    }

    private void Update()
    {
        killTimer += Time.deltaTime;
        if (!(killTimer >= life)) return;
        Explode(null);
        killTimer = 0;
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

        explosions.Explode(explosionName, transform.position, 1f);
        gameObject.SetActive(false);
    }
}