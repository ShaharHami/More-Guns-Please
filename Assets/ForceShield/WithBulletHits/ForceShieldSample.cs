using UnityEngine;

namespace ForceShield
{
    /// <summary>
    /// Adds hits on LMB to ForceShieldController using raycast 
    /// </summary>
    public class ForceShieldSample : MonoBehaviour
    {
        [SerializeField] float _radius = 1;
        [SerializeField] float _duration = 0.5f;

        ForceShieldController _forceShieldController;


        void Awake()
        {
            _forceShieldController = GetComponent<ForceShieldController>();
        }

        public void HitShield(Vector3 point)
        {
            _forceShieldController.AddHit(point, _duration, _radius);
        }
    }
}