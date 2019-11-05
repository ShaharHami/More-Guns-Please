using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] float speed = 1f;
    [SerializeField] float tilt = 1f;
    void Start()
    {
        
    }

    void Update()
    {
        Flight();
        Tilt();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            print(touch);
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

    private void Tilt()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = joystick.Direction.x * -tilt * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rot);
        print(transform.rotation.eulerAngles);
    }
}
