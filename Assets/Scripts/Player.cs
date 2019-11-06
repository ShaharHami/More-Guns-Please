using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] ParticleSystem[] shots;
    [SerializeField] ParticleSystem[] engines;
    [SerializeField] float speed = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float speedEase = 1f;

    private Vector3 _speed;
    [SerializeField] float tilt = 1f;
    [Range(0.0f, 1.0f)] [SerializeField] float tiltEase = 1f;
    private Vector3 _tilt;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Screen.height));
        objectWidth = transform.GetComponent<MeshRenderer>().bounds.extents.x;
        objectHeight = transform.GetComponent<MeshRenderer>().bounds.extents.z;
    }

    void FixedUpdate()
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
            else if (touch.phase == TouchPhase.Ended)
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
    }

    private void Flight()
    {
        _speed = new Vector3(
                    joystick.Horizontal * speed * Time.deltaTime,
                    transform.position.y,
                    joystick.Vertical * speed * Time.deltaTime
                );

        Vector3 pos = transform.position + _speed;
        pos.x = Mathf.Clamp(pos.x + _speed.x, -screenBounds.x/7 + objectWidth, screenBounds.x/7 - objectWidth);
        pos.z = Mathf.Clamp(pos.z + _speed.z, screenBounds.y/10 + objectHeight, -screenBounds.y/10 - objectHeight);
        transform.position = pos;
    }
    void ControlEngineFlameLength()
    {
        foreach (ParticleSystem engine in engines)
        {
            Vector3 engineScale = engine.transform.localScale;
            engineScale.z = joystick.Vertical*speed + 20;
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
            transform.position += _speed;
            if (_tilt.sqrMagnitude < 0.01f * 0.01f) _tilt = Vector3.zero;
        }
    }

    private void Shoot(bool shoot)
    {
        if (shoot)
        {
            foreach (ParticleSystem shot in shots)
            {
                shot.Play();
            }
        }
        else
        {
            foreach (ParticleSystem shot in shots)
            {
                shot.Stop();
            }
        }
    }
}
