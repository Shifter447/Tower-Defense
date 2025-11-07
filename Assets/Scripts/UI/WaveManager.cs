using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Types")]
    public GameObject missileEnemyPrefab;  // "Enemy"
    public GameObject heavyEnemyPrefab;    // "Enemy_2"
    public GameObject bossEnemyPrefab;     // "BossEnemy"

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public float timeBetweenEnemies = 0.5f;

    [Header("Wave Settings")]
    public int startEnemies = 5;
    public float difficultyMultiplier = 1.25f;
    public float timeBetweenWaves = 5f;
    public int finalWave = 15;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI enemiesAliveText;

    private int currentWave = 0;
    private bool spawning = false;
    private float countdown = 0f;

    void Start()
    {
        countdown = 10f;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();

        if (spawning) return;
        if (EnemiesAlive() > 0) return;

        if (currentWave >= finalWave) return;

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave()
    {
        spawning = true;
        currentWave++;

        if (waveText != null)
            waveText.text = $"Wave {currentWave}";

        // Boss wave
        if (currentWave == finalWave)
        {
            yield return StartCoroutine(SpawnBossWave());
            spawning = false;
            yield break;
        }

        // Normal waves
        int totalEnemies = Mathf.RoundToInt(startEnemies * Mathf.Pow(difficultyMultiplier, currentWave - 1));

        int heavyCount = 0;
        if (currentWave >= 5)
            heavyCount = Mathf.FloorToInt(totalEnemies * 0.25f);

        int normalCount = totalEnemies - heavyCount;

        List<GameObject> spawnQueue = GenerateMixedSpawnList(missileEnemyPrefab, heavyEnemyPrefab, normalCount, heavyCount);

        foreach (GameObject prefab in spawnQueue)
        {
            SpawnEnemy(prefab);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        spawning = false;
    }

    IEnumerator SpawnBossWave()
    {
        if (bossEnemyPrefab == null)
        {
            Debug.LogError("Boss prefab not assigned!");
            yield break;
        }

        if (waveText != null)
            waveText.text = "FINAL WAVE: BOSS";

        yield return new WaitForSeconds(2f);

        SpawnEnemy(bossEnemyPrefab);
    }

    List<GameObject> GenerateMixedSpawnList(GameObject normal, GameObject heavy, int normalCount, int heavyCount)
    {
        List<GameObject> list = new List<GameObject>();
        int total = normalCount + heavyCount;
        int nextHeavyIndex = (heavyCount > 0) ? Mathf.Max(1, total / (heavyCount + 1)) : -1;

        for (int i = 0; i < total; i++)
        {
            if (heavyCount > 0 && i != 0 && i % nextHeavyIndex == 0)
            {
                list.Add(heavy);
                heavyCount--;
            }
            else
            {
                list.Add(normal);
            }
        }

        while (heavyCount > 0)
        {
            int randomIndex = Random.Range(0, list.Count);
            list.Insert(randomIndex, heavy);
            heavyCount--;
        }

        return list;
    }

    void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null) return;

        GameObject enemyGO = Instantiate(prefab, spawnPoint.position, prefab.transform.rotation);

        // Assign waypoints to any enemy type
        MissileEnemyMovement missile = enemyGO.GetComponent<MissileEnemyMovement>();
        if (missile != null)
            missile.waypoints = waypoints;

        HeavyEnemyMovement heavy = enemyGO.GetComponent<HeavyEnemyMovement>();
        if (heavy != null)
            heavy.waypoints = waypoints;

        BossEnemy boss = enemyGO.GetComponent<BossEnemy>();
        if (boss != null)
            boss.waypoints = waypoints;

        // Scale health for all enemies
        EnemyHealth hp = enemyGO.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.maxHealth *= Mathf.Pow(difficultyMultiplier, currentWave - 1);
            hp.currentHealth = hp.maxHealth;
        }

        if (prefab == bossEnemyPrefab && hp != null)
        {
            hp.maxHealth *= 5f;
            hp.currentHealth = hp.maxHealth;
        }
    }

    int EnemiesAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void UpdateUI()
    {
        if (countdownText != null && !spawning && EnemiesAlive() == 0 && currentWave < finalWave)
            countdownText.text = $"Next wave in: {countdown:0.0}s";
        else if (countdownText != null)
            countdownText.text = "";

        if (waveText != null)
        {
            if (currentWave < finalWave)
                waveText.text = $"Wave: {currentWave}";
            else
                waveText.text = "Final Wave!";
        }

        if (enemiesAliveText != null)
            enemiesAliveText.text = $"Enemies: {EnemiesAlive()}";
    }
}
