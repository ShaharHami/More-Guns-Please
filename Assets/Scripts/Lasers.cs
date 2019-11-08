using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] shots;
    void Update()
    {
        // TODO: replace with event from main player script
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
        else if (Input.GetMouseButton(0)) // Mouse support for debugging purposes
        {
            Shoot(true);
        }
        else
        {
            Shoot(false);
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
