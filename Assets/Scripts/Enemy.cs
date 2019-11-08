using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EventCallbacks
{
    public class Enemy : MonoBehaviour
    {
        public bool targetable = true;
        private int enemyHealth = 50;
        void Start()
        {
            MissleHitEvent.RegisterListener(OnMissleHit);
        }

        void OnDestroy()
        {
            MissleHitEvent.UnregisterListener(OnMissleHit);
        }

        void OnMissleHit(MissleHitEvent hit)
        {
            if (hit.UnitGO == gameObject)
            {
                targetable = true;
                Damage(hit.damage);
                // Debug.Log(hit.Description + hit.UnitGO.name + " And made " + hit.damage + " Damage.");
            }
        }
        void OnParticleCollision(GameObject other)
        {
            if(other.gameObject.CompareTag("Lasers"))
            {
                Damage(1); //TODO: get laser damage from central data manager
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
        }
        void KillEnemy()
        {
            Destroy(gameObject);
        }
    }
}
