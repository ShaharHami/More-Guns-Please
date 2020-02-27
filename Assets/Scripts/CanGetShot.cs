using UnityEngine;
using EventCallbacks;

public class CanGetShot : MonoBehaviour
{
    private void OnEnable()
    {
        MissleHitEvent.RegisterListener(OnMissleHit);
    }

    void OnMissleHit(MissleHitEvent hit)
    {
        if (hit.UnitGO == gameObject)
        {
//            Damage(hit.damage);
        }
    }
}
