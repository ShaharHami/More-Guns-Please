using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

[RequireComponent(typeof(DoABarrelRoll))]
public class DragFingerMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float motionDampRate = 5f;
    public float rotationDampRate = 5f;
    public float maxSpeed;
    [SerializeField] private float zOffset;
    public float zOffsetChangeFactor;
    public Transform[] targetsToRoll;
    [SerializeField] float tilt = 1f;
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
    private float variableZOffset;

    private Vector3 mousePos;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        doABarrelRoll = GetComponent<DoABarrelRoll>();
    }

    private void FixedUpdate()
    {
        oldPos = inputPos;
        barrelRollThreshold = Screen.width / doABarrelRoll.threshold;
        thresholdDisplay.text = doABarrelRoll.threshold.ToString();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            HandleFlight();
        }
        else if (Input.GetMouseButton(0))
        {
            inputPos = Input.mousePosition;
            HandleFlight();
        }
        else
        {
            timer = 0;
            inputPos = transform.position;
            EaseOut();
        }

        zOffsetDisplay.text = variableZOffset.ToString();
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
        doABarrelRoll.threshold += 0.5f;
    }

    public void DownThreshold()
    {
        doABarrelRoll.threshold -= 0.5f;
        if (doABarrelRoll.threshold <= 1)
        {
            doABarrelRoll.threshold = 1;
        }
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
        float modifier = scale(0, Screen.height, 0, 1, inputPos.y) * Time.fixedDeltaTime;
        variableZOffset = zOffset + zOffsetChangeFactor * modifier;
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
        foreach (var t in targetsToRoll)
        {
            t.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, t.rotation.eulerAngles.z),
                Quaternion.Euler(0, 0, 0), rotationDampRate * Time.deltaTime);
        }

        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, motionDampRate * Time.deltaTime);
    }

    private void Move(Vector3 inputPosition)
    {
        inputPosition.z = mainCam.transform.position.y;
        mousePos = mainCam.ScreenToWorldPoint(inputPosition);
        direction = new Vector3(
            mousePos.x - transform.position.x,
            0,
            mousePos.z + variableZOffset - transform.position.z
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
        if (doABarrelRoll.isRotating) return;
        foreach (var t in targetsToRoll)
        {
            Vector3 _tilt = new Vector3 {x = 0, y = 0, z = (mousePos.x - t.position.x) * -tilt * Time.deltaTime};
            t.rotation = Quaternion.Euler(_tilt);
        }
    }

    private void HandleBarrelRoll()
    {
        float amountX = inputPos.x - oldPos.x;
        if (Math.Abs(amountX) > barrelRollThreshold)
        {
            doABarrelRoll.BarrelRoll(transform, Math.Sign(inputPos.x - oldPos.x) * -1);
        }
    }

    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}