using System.Linq;
using UnityEngine;
using EventCallbacks;
using ForceShield;

public class CollisionHandler : MonoBehaviour
{
    public string[] shooterTags;
    int counter;
    string message;
    private ForceShieldSample forceShieldSample;

    private void Awake()
    {
        forceShieldSample = GetComponent<ForceShieldSample>();
    }

    private void OnEnable()
    {
        ProjectileHitEvent.RegisterListener(OnDamage);
    }

    void OnDisable()
    {
        ProjectileHitEvent.UnregisterListener(OnDamage);
    }

    private void OnDamage(ProjectileHitEvent hit)
    {
        if (!shooterTags.Contains(hit.shooter.tag)) return;
        // if (forceShieldSample != null)
        // {
        //     forceShieldSample.HitShield(hit.projectile.position);
        // }

        hit.target.GetComponent<IDamagable>().Damage(hit.projectile.GetComponent<ICauseDamage>().damageAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!shooterTags.Contains(other.gameObject.tag)) return;
        // if (forceShieldSample != null)
        // {
        //     forceShieldSample.HitShield(GetComponent<Collider>().ClosestPoint(other.transform.position));
        // }

        gameObject.GetComponent<IDamagable>().Damage(other.GetComponent<ICauseDamage>().damageAmount);
    }
}