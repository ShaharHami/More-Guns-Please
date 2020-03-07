using EventCallbacks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private Formation[] formations;
    private Player player;
    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        formations = enemySpawner.formations;
        player = FindObjectOfType<Player>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnWave();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            KillAllEnemies();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUpShots();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Enemy randEnemy =
                ObjectPooler.Instance.SpawnFromPool("Trident", Vector3.zero, Quaternion.Euler(0,180,0)).GetComponent<Enemy>();
            randEnemy.transform.position = new Vector3(Random.Range(-50f, 50f), 0, 30);
            randEnemy.TurnSpeed = 0.01f;
        }
    }

    public void LevelUpShots()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        foreach (Shot shot in player.shots)
        {
            if (shot.type == player.activeShot)
            {
                shot.LevelUp();
            }
        }
    }

    public void SpawnWave()
    {
        enemySpawner.SpawnFormation(formations[Random.Range(0, formations.Length)]);
    }

    public void KillAllEnemies()
    {
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            enemy.KillEnemy();
        }
    }
}
