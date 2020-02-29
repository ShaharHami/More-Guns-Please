using UnityEngine;
using Random = UnityEngine.Random;

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
        private TrailRenderer trailRenderer;
        private float killTimer;

        void Awake()
        {
            initialTarget = target;
            trailRenderer = GetComponent<TrailRenderer>();
            explosions = FindObjectOfType<Explosions>();
            launcher = FindObjectOfType<Launcher>();
        }

        private void OnEnable()
        {
            target = launcher.GetTarget(this);
        }

        private void OnDisable()
        {
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Explode();
            }
        }
        private void FixedUpdate()
        {
            if (!target || !target.gameObject.activeSelf)
            {
                target = initialTarget;
            }
            else
            {
                direction = target.position - transform.position;
                direction.y = 0;
                direction.Normalize();
                transform.LookAt(target);
            }
            Vector3 motion = Time.fixedDeltaTime * speed * direction;
            motion.y = 0;
            transform.position += motion;
            killTimer += Time.fixedDeltaTime;
            if (killTimer >= life)
            {
                Explode();
                killTimer = 0;
            }
        }

        private void Explode()
        {
            if (gameObject.activeSelf)
            {
                if (target != null)
                {
                    MissleHitEvent missileHit = new MissleHitEvent();
                    missileHit.Description = "Unit " + gameObject.name + " has hit ";
                    missileHit.UnitGO = target.gameObject;
                    missileHit.damage = Random.Range(1, 5); //TODO: get rocket damage from central data manager
                    missileHit.FireEvent();
                }
                explosions.Explode("missile hit", transform.position, 1f);
                target = null;
                gameObject.SetActive(false);
            }
        }
    }
}