using UnityEngine;
using EventCallbacks;

public class CanGetShot : MonoBehaviour
{
    private void OnEnable()
    {
        ProjectileHitEvent.RegisterListener(OnMissleHit);
    }

    void OnMissleHit(ProjectileHitEvent hit)
    {
        if (hit.shooter == gameObject)
        {
//            Damage(hit.damage);
        }
    }
}
