using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [SerializeField] private float speed = 100f;
    private void OnCollisionEnter(Collision other)
    {
        // print(other.collider.name);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction;
            direction = target.position - transform.position;
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
        else {
            print ("no target");
        }
    }
}
