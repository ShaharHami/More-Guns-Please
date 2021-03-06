﻿using System;
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
    private FireInputManager fireInputManager;
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
    public int upgradeCount;
    private SceneManager sceneManager;
    private bool volleyActive;
    private Coroutine nextWaveCoroutine, respawnCoroutine;

    private void OnEnable()
    {
        PlayerDied.RegisterListener(OnPlayerDeath);
        FormationDead.RegisterListener(OnFormationDead);
        UpgradeEvent.RegisterListener(OnUpgraded);
        gameOver.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerDied.UnregisterListener(OnPlayerDeath);
        FormationDead.UnregisterListener(OnFormationDead);
        UpgradeEvent.UnregisterListener(OnUpgraded);
        StopAllCoroutines();
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        formations = enemySpawner.formations;
        player = FindObjectOfType<Player>();
        fireInputManager = player.GetComponent<FireInputManager>();
        respawnCountdownDisplay.transform.position = Camera.main.WorldToScreenPoint(player.spawnPoint);
        respawnCountdownDisplay.gameObject.SetActive(false);
        upgradeMessage.SetActive(false);
        livesDisplay.text = lives.ToString();
        wavesDisplay.text = wavesCounter + " / " + maxWaves;
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
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeManager.Instance.SpawnUpgrades(upgradeCount);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            volleyActive = !volleyActive;
            fireInputManager.SetAutoFire(volleyActive);
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

    public void StartGame()
    {
        startButton.SetActive(false);
        Invoke(nameof(StartShield), 1f);
        Invoke(nameof(StartWaves), 2f);
    }

    private void StartWaves()
    {
        nextWaveCoroutine = StartCoroutine(NextWaveCountdown());
    }

    private void StartShield()
    {
        player.forceShieldController.EnableShield();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnWave()
    {
        if (simultaniuosWavesCounter >= allowedSimultaniousWaves) return;
        enemySpawner.SpawnFormation(formations[Random.Range(0, formations.Length)]);
        simultaniuosWavesCounter++;
        wavesCounter++;
        wavesDisplay.text = wavesCounter + " / " + maxWaves;
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
        livesDisplay.text = lives.ToString();
        if (lives <= 0)
        {
            HandleGameOver(false);
        }
        else
        {
            respawnCountdownDisplay.gameObject.SetActive(true);
            respawnCoroutine = StartCoroutine(Respawn(info.respawnTimer));
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
            upgradeMessage.SetActive(true);
            UpgradeManager.Instance.SpawnUpgrades(upgradeCount);
        }
        else
        {
            HandleGameOver(true);
        }
    }

    private void OnUpgraded(UpgradeEvent e)
    {
        upgradeMessage.SetActive(false);
        nextWaveCoroutine = StartCoroutine(NextWaveCountdown());
    }
    private IEnumerator NextWaveCountdown()
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