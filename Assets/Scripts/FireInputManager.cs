using UnityEngine;

public class FireInputManager : MonoBehaviour
{
    public float threshold;
    private bool autoFire;
    private bool shooting;
    private float timer;
    private bool startTimer, canShoot;

    public bool FireInput()
    {
        if (autoFire)
        {
            shooting = true;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (canShoot)
                        {
                            shooting = true;
                            startTimer = false;
                        }

                        break;
                    case TouchPhase.Ended:
                        if (canShoot)
                        {
                            timer = 0f;
                            canShoot = false;
                            shooting = false;
                        }

                        break;
                }
            }

            if (Input.GetMouseButtonUp(0) && canShoot)
            {
                timer = 0;
                canShoot = false;
            }
            shooting = Input.GetMouseButton(0) && canShoot;
        }

        return shooting;
    }

    private void Update()
    {
        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= threshold)
            {
                canShoot = true;
            }
        }
    }

    public void SetAutoFire(bool setAutoFire)
    {
        autoFire = setAutoFire;
    }
}