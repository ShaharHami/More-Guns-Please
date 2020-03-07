using System;
using UnityEngine;
using EventCallbacks;

public class Player : MonoBehaviour
{
    public Joystick joystick;
    public int playerHealth = 100;
    public Shot[] shots;
    [SerializeField] ParticleSystem[] engines;
    private float minEngineScaleZ;
    [SerializeField] float speed = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float speedEase = 1f;

    private Vector3 _speed;
    [SerializeField] float tilt = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float tiltEase = 1f;
    [Range(0.0f, 0.5f)] public float marginLeft = 0f, marginRight = 0f, marginTop = 0f, marginBottom = 0f;
    private Vector3 _tilt;
    private bool flying;
    private bool shooting;
    public float barrelRollThreshold;
    private float joystickX;
    private DoABarrelRoll doABarrelRoll;
    public string activeShot { get; private set; }
    private Camera camera1;
    private Vector3 viewpointCoord;

    void Start()
    {
        camera1 = Camera.main;
        doABarrelRoll = GetComponent<DoABarrelRoll>();
        if (engines[0] != null)
        {
            minEngineScaleZ = engines[0].transform.localScale.z;
        }

        EnemyShotHit.RegisterListener(OnDamage);
        activeShot = shots[0].type;
        foreach (Shot shot in shots)
        {
            if (shot.type != activeShot)
            {
                shot.SetInactive();
            }
            else
            {
                shot.SetLevel(0);
            }
        }
    }

    void Update()
    {
        ControlEngineFlameLength();
        if (Input.touchCount > 0)
        {
            flying = true;
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                shooting = true;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                shooting = false;
            }
        }

        if (Input.GetMouseButton(0)) // Mouse support for debugging purposes
        {
            flying = true;
            shooting = true;
        }
        else
        {
            flying = false;
            shooting = false;
        }

        if (Input.GetKey(KeyCode.Space)) // keyboard support for debugging purposes
        {
            shooting = true;
            flying = true;
        }
        if (shooting)
        {
            Shoot(!doABarrelRoll.isRotating);
        }
        else
        {
            Shoot(false);
        }
    }

    private void FixedUpdate()
    {
        if (flying)
        {
            HandleBarrelRoll();
            Flight();
            Roll();
        }
        else
        {
            joystickX = 0;
            EaseOut();
        }
    }

    void HandleBarrelRoll()
    {
        float currentX = joystick.Direction.x;
        if (Math.Abs(currentX) > barrelRollThreshold && Math.Abs(joystickX) > barrelRollThreshold)
        {
            if (Math.Sign(joystickX) != Math.Sign(currentX))
            {
                doABarrelRoll.BarrelRoll(transform, 1 * Math.Sign(joystickX));
            }
        }
        joystickX = currentX;
        if (Math.Abs(Input.GetAxis("Horizontal")) > 0 && Input.GetKey(KeyCode.LeftControl))
        {
            doABarrelRoll.BarrelRoll(transform, -1 * Math.Sign(Input.GetAxis("Horizontal")));
        }
    }

    private void Flight()
    {
        _speed = new Vector3(
            (joystick.Horizontal + Input.GetAxis("Horizontal")) * speed * Time.deltaTime,
            0,
            (joystick.Vertical + Input.GetAxis("Vertical")) * speed * Time.deltaTime
        );
        viewpointCoord = camera1.WorldToViewportPoint(transform.position);
        viewpointCoord.x = Mathf.Clamp(viewpointCoord.x, 0f + marginLeft, 1f - marginRight);
        viewpointCoord.y = Mathf.Clamp(viewpointCoord.y, 0f + marginBottom, 1f - marginTop);
        transform.position = camera1.ViewportToWorldPoint(viewpointCoord) + _speed;
    }

    void ControlEngineFlameLength()
    {
        foreach (ParticleSystem engine in engines)
        {
            Vector3 engineScale = engine.transform.localScale;
            engineScale.z = (joystick.Vertical + Input.GetAxis("Vertical")) * speed + minEngineScaleZ;
            if (engineScale.z < minEngineScaleZ)
            {
                engineScale.z = minEngineScaleZ;
            }

            engine.transform.localScale = engineScale;
        }
    }

    private void Roll()
    {
        _tilt = transform.rotation.eulerAngles;
        _tilt.z = (joystick.Direction.x + Input.GetAxis("Horizontal")) * -tilt * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_tilt);
    }

    private void EaseOut()
    {
        if (_tilt.sqrMagnitude > 0f)
        {
            _tilt *= tiltEase;
            _speed *= speedEase;
            transform.rotation = Quaternion.Euler(_tilt);
            if (_tilt.sqrMagnitude < 0.01f * 0.01f) _tilt = Vector3.zero;
            var worldToViewportPoint = camera1.WorldToViewportPoint(transform.position);
            if (worldToViewportPoint.x > (0f + marginLeft) && worldToViewportPoint.x < (1f - marginRight) &&
                worldToViewportPoint.y > (0f + marginBottom) && worldToViewportPoint.y < (1f - marginTop))
            {
                transform.position += _speed;
            }
        }
    }

    private void OnDamage(EnemyShotHit hit)
    {
        if (doABarrelRoll.isRotating)
        {
            print($"woohoo got away from {hit.UnitGO.name}!");
            return;
        }
        print("Player got hit by " + hit.UnitGO.name + " and lost " + hit.damage + " Points of health");
    }

    private void Damage(int damage)
    {
//        print("Player hit" + damage);
    }

    void OnDestroy()
    {
        EnemyShotHit.UnregisterListener(OnDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (doABarrelRoll.isRotating) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            Damage(playerHealth);
        }
    }

    private void Shoot(bool fire)
    {
        FireWeapon fireWeaponEvent = new FireWeapon();
        fireWeaponEvent.Description = "Fire weapon: " + fire;
        fireWeaponEvent.fire = fire;
        fireWeaponEvent.FireEvent();
    }
}