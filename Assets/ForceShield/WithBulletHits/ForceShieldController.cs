using System;
using UnityEngine;
using DG.Tweening;

namespace ForceShield
{
    /// <summary>
    /// Stores hits and sends them to renderer
    /// </summary>
    public class ForceShieldController : MonoBehaviour, IDamagable
    {
        public float on, off;
        [SerializeField, Range(-10, 10)] float _DissolveValue;
        [SerializeField, Range(0, 10)] float _animationDurationIN = 2;
        [SerializeField, Range(0, 10)] float _animationDurationOUT = 2;

        public int health { get; set; }
        public int maxHealth { get; set; }
        public int shieldMaxHealth = 100;
        [Range(0, 1)] public float shieldLevel;

        [ColorUsage(true, true)] public Color fullShieldColor, emptyShieldColor;
        const int MAX_HITS_COUNT = 10;

        Renderer _renderer;
        MaterialPropertyBlock _mpb;

        int _hitsCount;
        Vector4[] _hitsObjectPosition = new Vector4[MAX_HITS_COUNT];
        float[] _hitsDuration = new float[MAX_HITS_COUNT];
        float[] _hitsTimer = new float[MAX_HITS_COUNT];
        float[] _hitRadius = new float[MAX_HITS_COUNT];

        //1(max)..0(end of life time)
        float[] _hitsIntensity = new float[MAX_HITS_COUNT];

        bool _recordingPlayed;
        float _startTime;
        float _finishTime;
        float _targetDissolveValue;
        [HideInInspector] public HealthDisplay healthDisplay;
        public ShieldUpgradeManager shieldUpgradeManager;
        private static readonly int IntersectionColor = Shader.PropertyToID("_intersectionColor");
        private static readonly int DissolveValue = Shader.PropertyToID("_DissolveValue");
        private static readonly int HitsCount = Shader.PropertyToID("_HitsCount");
        private static readonly int HitsRadius = Shader.PropertyToID("_HitsRadius");
        private static readonly int HitsObjectPosition = Shader.PropertyToID("_HitsObjectPosition");
        private static readonly int HitsIntensity = Shader.PropertyToID("_HitsIntensity");

        public void AddHit(Vector3 worldPosition, float duration, float radius)
        {
            int id = GetFreeHitId();
            _hitsObjectPosition[id] = transform.InverseTransformPoint(worldPosition);
            _hitsDuration[id] = duration;
            _hitRadius[id] = radius;

            _hitsTimer[id] = 0;
        }

        int GetFreeHitId()
        {
            if (_hitsCount < MAX_HITS_COUNT)
            {
                _hitsCount++;
                return _hitsCount - 1;
            }
            else
            {
                float minDuration = float.MaxValue;
                int minId = 0;
                for (int i = 0; i < MAX_HITS_COUNT; i++)
                {
                    if (_hitsDuration[i] < minDuration)
                    {
                        minDuration = _hitsDuration[i];
                        minId = i;
                    }
                }

                return minId;
            }
        }

        public void ClearAllHits()
        {
            _hitsCount = 0;
            SendHitsToRenderer();
        }

        public void PlayAppearingAnimation()
        {
            DOTween.To(() => _DissolveValue, x => _DissolveValue = x, on, _animationDurationIN).SetEase(Ease.OutCirc);
            _recordingPlayed = true;
            _finishTime = Time.time + Mathf.Lerp(0, _animationDurationIN, _DissolveValue);
            _targetDissolveValue = on;
        }

        public void PlayDisappearingAnimation()
        {
            DOTween.To(() => _DissolveValue, x => _DissolveValue = x, off, _animationDurationOUT);
            _recordingPlayed = true;
            _finishTime = Time.time + Mathf.Lerp(0, _animationDurationOUT, 1 - _DissolveValue);
            _targetDissolveValue = off;
        }

        private void Awake()
        {
            health = maxHealth = shieldMaxHealth;
            healthDisplay = GetComponent<HealthDisplay>();
            healthDisplay.SetHealth(maxHealth);
        }

        void OnEnable()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
            shieldLevel = (float) health / maxHealth;
            _DissolveValue = off;
            healthDisplay.ChangeHealth(health);
        }

        void Update()
        {
            // UpdateAnimation();
            UpdateHitsLifeTime();
            SendHitsToRenderer();
            UpdateColor();
        }

        public void Damage(int damage)
        {
            health -= damage;
            shieldLevel = (float) health / maxHealth;
            if (shieldLevel <= 0)
            {
                shieldLevel = 0;
                health = 0;
                PlayDisappearingAnimation();
                Invoke(nameof(DisableShield), _animationDurationOUT);
            }
            healthDisplay.ChangeHealth(health);
            shieldUpgradeManager.MaxedOut = health >= maxHealth;
        }

        public void DisableShield()
        {
            gameObject.SetActive(false);
        }

        public void EnableShield()
        {
            gameObject.SetActive(true);
            PlayAppearingAnimation();
        }

        public void SetShieldHealth(int shieldHealth)
        {
            health = shieldHealth;
            shieldLevel = (float) health / maxHealth;
        }

        void UpdateColor()
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(IntersectionColor, Color.Lerp(emptyShieldColor, fullShieldColor, shieldLevel));
            _renderer.SetPropertyBlock(_mpb);
        }

        void UpdateHitsLifeTime()
        {
            for (int i = 0; i < _hitsCount;)
            {
                _hitsTimer[i] += Time.deltaTime;
                if (_hitsTimer[i] > _hitsDuration[i])
                {
                    SwapWithLast(i);
                }
                else
                {
                    i++;
                }
            }
        }

        void SwapWithLast(int id)
        {
            int idLast = _hitsCount - 1;
            if (id != idLast)
            {
                _hitsObjectPosition[id] = _hitsObjectPosition[idLast];
                _hitsDuration[id] = _hitsDuration[idLast];
                _hitsTimer[id] = _hitsTimer[idLast];
                _hitRadius[id] = _hitRadius[idLast];
            }

            _hitsCount--;
        }

        // void UpdateAnimation()
        // {
        //     if (_recordingPlayed)
        //     {
        //         _DissolveValue = Mathf.Lerp(1 - _targetDissolveValue, _targetDissolveValue,
        //             Mathf.InverseLerp(_finishTime - _animationDuration, _finishTime, Time.time));
        //         if (Time.time > _finishTime)
        //         {
        //             _recordingPlayed = false;
        //         }
        //     }
        // }


        void SendHitsToRenderer()
        {
            _renderer.GetPropertyBlock(_mpb);

            _mpb.SetFloat(DissolveValue, _DissolveValue);
            _mpb.SetFloat(HitsCount, _hitsCount);
            _mpb.SetFloatArray(HitsRadius, _hitRadius);

            for (int i = 0; i < _hitsCount; i++)
            {
                if (_hitsDuration[i] > 0f)
                {
                    _hitsIntensity[i] = 1 - Mathf.Clamp01(_hitsTimer[i] / _hitsDuration[i]);
                }
            }

            _mpb.SetVectorArray(HitsObjectPosition, _hitsObjectPosition);
            _mpb.SetFloatArray(HitsIntensity, _hitsIntensity);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}