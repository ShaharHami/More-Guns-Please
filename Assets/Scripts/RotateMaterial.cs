using UnityEngine;
using Random = UnityEngine.Random;

public class RotateMaterial : MonoBehaviour
{
    public Renderer renderer;
    [MinMax(0, 1000f)] public Vector2 rotationSpeedRange;
    private float rotationSpeed;
    private float sign;

    private void OnEnable()
    {
        sign = Random.Range(0f, 1f) > 0.5f ? Mathf.Sign(1) : Mathf.Sign(-1);
        rotationSpeed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y) * sign;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + (rotationSpeed * Time.deltaTime), 0));
        transform.rotation = rotation;
    }
}
