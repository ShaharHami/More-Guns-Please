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
        [HideInInspector] public Launcher launcher;
        Vector3 direction;
        private Explosions explosions;
        void Awake()
        {
            initialTarget = target;
        }
        private void Start()
        {
            Invoke("Explode", life);
            launcher.GetTarget(this);
            explosions = FindObjectOfType<Explosions>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Enemy>() != null)
            {
                Explode();
            }
        }
        void Update()
        {
            if (target == null)
            {
                target = initialTarget;
                launcher.GetTarget(this);
                return;
            }
            else
            {
                direction = target.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                transform.LookAt(target);
            }
        }
        private void FixedUpdate()
        {
            Vector3 motion = direction * speed * Time.deltaTime;
            motion.y = 0;
            transform.position += motion;
        }

        private void Explode()
        {
            if (target != null)
            {
                MissleHitEvent missileHit = new MissleHitEvent();
                missileHit.Description = "Unit " + gameObject.name + " has hit ";
                missileHit.UnitGO = target.gameObject;
                missileHit.damage = Random.Range(1, 5); //TODO: get rocket damage from central data manager
                missileHit.FireEvent();
            }
            explosions.Explode("Missile Hit", transform.position, 1f);
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
