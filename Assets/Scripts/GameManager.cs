using EventCallbacks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private Formation[] formations;
    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        formations = enemySpawner.formations;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemySpawner.SpawnFormation(formations[Random.Range(0, formations.Length)], new []{"Trident"});
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Enemy randEnemy =
                ObjectPooler.Instance.SpawnFromPool("Trident", Vector3.zero, Quaternion.identity).GetComponent<Enemy>();
            randEnemy.transform.position = new Vector3(Random.Range(-50f, 50f), 0, 100);
        }
    }
}
