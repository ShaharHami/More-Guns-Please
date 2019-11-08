using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public class Rocket : MonoBehaviour
    {
        public Transform target;
        public Transform initialTarget;
        [SerializeField] private float speed = 100f;
        [SerializeField] private float life = 2f;
        private Launcher launcher;
        Vector3 direction;
        void Awake()
        {
            initialTarget = target;
        }
        private void Start()
        {
            Invoke("Explode", life);
            launcher = FindObjectOfType<Launcher>();
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.GetComponent<Enemy>() != null)
            {
                Explode();
            }
        }

        private void FixedUpdate()
        {

            if (target != null)
            {
                direction = target.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                transform.LookAt(target);
            }
            else
            {
                launcher.GetTarget(this);
            }
            transform.position += direction * speed * Time.deltaTime;
        }

        private void Explode()
        {
            if (target != null)
            {
                MissleHitEvent missileHit = new MissleHitEvent();
                missileHit.Description = "Unit " + gameObject.name + " has hit ";
                missileHit.UnitGO = target.gameObject;
                missileHit.damage = Random.Range(0, 5); //TODO: get rocket damage from central data manager
                missileHit.FireEvent();
            }

            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
