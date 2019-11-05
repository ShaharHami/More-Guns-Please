using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] ParticleSystem shots;
    [SerializeField] float speed = 1f;
    [SerializeField] float tilt = 1f;
    void Start()
    {

    }

    void FixedUpdate()
    {
        Flight();
        Roll();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Shoot(true);
            } 
            else if (touch.phase == TouchPhase.Ended)
            {
                Shoot(false);
            }
        }
    }

    private void Flight()
    {
        transform.position += new Vector3(
                    joystick.Direction.x * speed * Time.deltaTime,
                    transform.position.y,
                    joystick.Direction.y * speed * Time.deltaTime
                );
    }

    private void Roll()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = joystick.Direction.x * -tilt * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rot);
    }

    private void Shoot(bool shoot)
    {
        if (shoot)
        {
            shots.Play();
        }
        else
        {
            shots.Stop();
        }
    }
}
