using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private Formation[] formations;
    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        formations = enemySpawner.formations;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemySpawner.SpawnFormation(formations[Random.Range(0, formations.Length)], new []{"Trident"});
        }
    }
}
