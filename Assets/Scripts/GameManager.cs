using System;
using System.Collections;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private EnemySpawner enemySpawner;
    private Formation[] formations;
    private Player player;
    public TextMeshProUGUI respawnCountdownDisplay;
    public TextMeshProUGUI livesDisplay;
    public TextMeshProUGUI wavesDisplay;
    public GameObject upgradeMessage;
    public int lives;
    public TextMeshProUGUI gameOverDisplay;
    public GameObject gameOver;
    public int maxWaves;
    private int wavesCounter;
    public int allowedSimultaniousWaves;
    private int simultaniuosWavesCounter;
    public float delayBetweenWaves;
    public GameObject startButton;
    private SceneManager sceneManager;
    private int shotLevels;
    private int upgrades;
    private bool volleyActive;

    private void OnEnable()
    {
        PlayerDied.RegisterListener(OnPlayerDeath);
        FormationDead.RegisterListener(OnFormationDead);
        gameOver.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerDied.UnregisterListener(OnPlayerDeath);
        FormationDead.UnregisterListener(OnFormationDead);
    }

    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        formations = enemySpawner.formations;
        player = FindObjectOfType<Player>();
        shotLevels = player.shots[0].shotLevelTransforms.Length-1;
        respawnCountdownDisplay.transform.position = Camera.main.WorldToScreenPoint(player.spawnPoint);
        respawnCountdownDisplay.gameObject.SetActive(false);
        upgradeMessage.SetActive(false);
        livesDisplay.text = lives.ToString();
        wavesDisplay.text = wavesCounter + " / " + maxWaves;
    }

    private void ActivateVolley()
    {
        var volley = FindObjectOfType<Volley>();
        volleyActive = !volleyActive;
        volley.PerformSimpleVolley(volleyActive);
    }

    private IEnumerator AutoVolley()
    {
        yield return new WaitForSeconds(1f);
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
        if (Input.GetKeyDown(KeyCode.V))
        {
            ActivateVolley();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Enemy randEnemy =
                ObjectPooler.Instance.SpawnFromPool("Trident", Vector3.zero, Quaternion.Euler(0, 180, 0))
                    .GetComponent<Enemy>();
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

    public void StartGame()
    {
        StartCoroutine(NextWaveCountdown());
        startButton.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void SpawnWave()
    {
        if (simultaniuosWavesCounter < allowedSimultaniousWaves)
        {
            enemySpawner.SpawnFormation(formations[Random.Range(0, formations.Length)]);
            simultaniuosWavesCounter++;
            wavesCounter++;
            wavesDisplay.text = wavesCounter + " / " + maxWaves;
        }
    }

    public void KillAllEnemies()
    {
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            enemy.KillEnemy();
        }
    }

    void OnPlayerDeath(PlayerDied info)
    {
        lives--;
        upgrades = 0;
        livesDisplay.text = lives.ToString();
        if (lives <= 0)
        {
            HandleGameOver(false);
        }
        else
        {
            respawnCountdownDisplay.gameObject.SetActive(true);
            StartCoroutine(Respawn(info.respawnTimer));
        }
    }

    IEnumerator Respawn(float respawnDelay)
    {
        for (int i = 0; i < respawnDelay; i++)
        {
            respawnCountdownDisplay.text = (respawnDelay - i).ToString();
            yield return new WaitForSeconds(1f);
        }

        respawnCountdownDisplay.gameObject.SetActive(false);
        player.enabled = true;
        player.gameObject.SetActive(true);
        respawnCountdownDisplay.transform.position = Camera.main.WorldToScreenPoint(player.spawnPoint);
    }

    void OnFormationDead(FormationDead info)
    {
        simultaniuosWavesCounter--;
        if (wavesCounter < maxWaves)
        {
            if (upgrades < shotLevels)
            {
                LevelUpShots();
                upgradeMessage.SetActive(true);
                upgrades++;
            }
            StartCoroutine(NextWaveCountdown());
        }
        else
        {
            HandleGameOver(true);
        }
    }

    IEnumerator NextWaveCountdown()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        upgradeMessage.SetActive(false);
        SpawnWave();
    }

    private void HandleGameOver(bool win)
    {
        var message = win ? "You Win!" : "You Lose";
        gameOverDisplay.text = message;
        gameOver.SetActive(true);
    }
}