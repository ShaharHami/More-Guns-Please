using System.Collections;
using UnityEngine;

public class ForwardMotion : MonoBehaviour
{
    public float speed;
    public float killTimer;
    private Vector3 dir;
    private Coroutine coroutine;
    private Transform player;
    private Vector3 playerPos;
    private Quaternion rotation;
    private bool hasDirection;
    
    void OnEnable()
    {
        player = FindObjectOfType<Player>().transform;
        playerPos = player.position;
        coroutine = StartCoroutine(Kill());
        dir = transform.position - playerPos;
        hasDirection = false;
    }

    private void OnDisable()
    {
        hasDirection = false;
        StopCoroutine(coroutine);
    }
    
    void Update()
    {
        if (!hasDirection)
        {
            dir = transform.position - playerPos;
            hasDirection = true;
            rotation = Quaternion.LookRotation(dir);
            transform.rotation = rotation;
        }
        transform.position += Time.deltaTime * speed * dir.normalized;
    }
    IEnumerator Kill()
    {
        yield return new WaitForSeconds(killTimer);
        gameObject.SetActive(false);
    }
}
