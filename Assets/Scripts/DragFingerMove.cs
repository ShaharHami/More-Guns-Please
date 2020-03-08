using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(DoABarrelRoll))]
public class DragFingerMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float motionDampRate = 5f;
    public float rotationDampRate = 5f;
    public float maxSpeed;
    [SerializeField] float tilt = 1f;
    [SerializeField] private float zOffset;
    [SerializeField] private TextMeshProUGUI thresholdDisplay;
    [SerializeField] private TextMeshProUGUI zOffsetDisplay;
    private Vector3 inputPos; 
    private Vector3 touchPos;
    private Vector3 oldPos;
    private Rigidbody rb;
    private Vector3 direction;
    private float timer;
    private DoABarrelRoll doABarrelRoll;
    private float barrelRollThreshold;

    private Vector3 mousePos;
    private Camera mainCam;

    // Use this for initialization
    private void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        doABarrelRoll = GetComponent<DoABarrelRoll>();
        barrelRollThreshold = doABarrelRoll.threshold;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        oldPos = inputPos;
        thresholdDisplay.text = barrelRollThreshold.ToString();
        zOffsetDisplay.text = zOffset.ToString();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            HandleFlight();
        }
        else if (Input.GetMouseButton(0))
        {
            barrelRollThreshold = doABarrelRoll.threshold / 2.5f;
            inputPos = Input.mousePosition;
            HandleFlight();
        }
        else
        {
            timer = 0;
            inputPos = transform.position;
            EaseOut();
        }
    }

    protected void LateUpdate()
    {
        if (!GetComponent<DoABarrelRoll>().isRotating)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }
    public void UpThreshold()
    {
        barrelRollThreshold += 5;
    }

    public void DownThreshold()
    {
        barrelRollThreshold -= 5;
    }

    public void UpZOffset()
    {
        zOffset += 1;
    }
    public void DownZOffset()
    {
        zOffset -= 1;
    }
    private void HandleFlight()
    {
        Move(inputPos);
        Roll();
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            HandleBarrelRoll();
        }
    }
    private void EaseOut()
    {
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(0,0, transform.rotation.eulerAngles.z), Quaternion.Euler(0,0,0), rotationDampRate * Time.deltaTime);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, motionDampRate * Time.deltaTime);
    }

    private void Move(Vector3 inputPosition)
    {
        inputPosition.z = mainCam.transform.position.y;
        mousePos = mainCam.ScreenToWorldPoint(inputPosition);
        direction = new Vector3(
            mousePos.x - transform.position.x,
            0,
            mousePos.z + zOffset - transform.position.z
        );
        direction *= Time.deltaTime;
        if (Math.Abs(direction.x) >= maxSpeed)
        {
            direction.x = maxSpeed * Math.Sign(direction.x);
        }

        if (Math.Abs(direction.z) >= maxSpeed)
        {
            direction.z = maxSpeed * Math.Sign(direction.z);
        }
        rb.velocity = new Vector3(direction.x, 0, direction.z) * moveSpeed;
    }

    private void Roll()
    {
        Vector3 _tilt = transform.rotation.eulerAngles;
        _tilt.x = 0;
        _tilt.y = 0;
        _tilt.z = (mousePos.x - transform.position.x) * -tilt * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_tilt);
    }

    private void HandleBarrelRoll()
    {
        float amountX = inputPos.x - oldPos.x;
        if (Math.Abs(amountX) > barrelRollThreshold)
        {
            doABarrelRoll.BarrelRoll(transform, Math.Sign(inputPos.x - oldPos.x) * -1);
        }
    }
}