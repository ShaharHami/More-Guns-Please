using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool cheat;
    public Joystick joystick;
    public int playerHealth = 100;
    [SerializeField] Shot[] shots;
    [SerializeField] ParticleSystem[] engines;
    private float minEngineScaleZ;
    [SerializeField] float speed = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float speedEase = 1f;

    private Vector3 _speed;
    [SerializeField] float tilt = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float tiltEase = 1f;
    [Range(0.0f, 0.5f)]
    public float marginLeft = 0f, marginRight = 0f, marginTop = 0f, marginBottom = 0f;
    private Vector3 _tilt;
    
    private float objectWidth;
    private float objectHeight;
    private string activeShot;
    private Camera camera1;
    private Vector3 viewpointCoord;

    void Start()
    {
        camera1 = Camera.main;
        objectWidth = transform.GetComponent<MeshRenderer>().bounds.extents.x;
        objectHeight = transform.GetComponent<MeshRenderer>().bounds.extents.z;
        if (engines[0] != null)
        {
            minEngineScaleZ = engines[0].transform.localScale.z;
        }
        EventCallbacks.EnemyShotHit.RegisterListener(OnDamage);
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
            Flight();
            Roll();
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Shoot(true);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                EaseOut();
                Shoot(false);
            }
        }
        else if (Input.GetMouseButton(0)) // Mouse support for debugging purposes
        {
            Flight();
            Roll();
            Shoot(true);
        }
        else
        {
            EaseOut();
            Shoot(false);
        }
        // CHEAT SETTINGS
        if (cheat && Input.GetKeyDown(KeyCode.L))
        {
            foreach (Shot shot in shots)
            {
                if (shot.type == activeShot)
                {
                    shot.LevelUp();
                }
            }
        }
    }

    private void Flight()
    {

        _speed = new Vector3(
                    joystick.Horizontal * speed * Time.deltaTime,
                    0,
                    joystick.Vertical * speed * Time.deltaTime
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
            engineScale.z = joystick.Vertical * speed + minEngineScaleZ;
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
        _tilt.z = joystick.Direction.x * -tilt * Time.deltaTime;
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
            if (worldToViewportPoint.x > (0f + marginLeft) && worldToViewportPoint.x < (1f - marginRight) && worldToViewportPoint.y > (0f + marginBottom) && worldToViewportPoint.y < (1f - marginTop))
            {
                transform.position += _speed;
            }
        }
    }
    private void OnDamage(EventCallbacks.EnemyShotHit hit)
    {
//        print("Player got hit by " + hit.UnitGO.name + " and lost " + hit.damage + " Points of health");
    }
    private void Damage(int damage)
    {
//        print("Player hit" + damage);
    }
    void OnDestroy()
    {
        EventCallbacks.EnemyShotHit.UnregisterListener(OnDamage);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Damage(playerHealth);
        }
    }
    private void Shoot(bool fire)
    {
        EventCallbacks.FireWeapon fireWeaponEvent = new EventCallbacks.FireWeapon();
        fireWeaponEvent.Description = "Fire weapon: " + fire;
        fireWeaponEvent.fire = fire;
        fireWeaponEvent.FireEvent();
    }
}
