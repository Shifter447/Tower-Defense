using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Types")]
    public GameObject normalEnemyPrefab;  // "Enemy"
    public GameObject heavyEnemyPrefab;   // "Enemy_2"

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public float timeBetweenEnemies = 0.5f;

    [Header("Wave Settings")]
    public int startEnemies = 5;
    public float difficultyMultiplier = 1.25f;
    public float timeBetweenWaves = 5f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI enemiesAliveText;

    private int currentWave = 0;
    private bool spawning = false;
    private float countdown = 0f;

    void Start()
    {
        countdown = 3f;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();

        if (spawning) return;
        if (EnemiesAlive() > 0) return;

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

        int totalEnemies = Mathf.RoundToInt(startEnemies * Mathf.Pow(difficultyMultiplier, currentWave - 1));
        int heavyCount = Mathf.Max(0, currentWave - 4);
        heavyCount = Mathf.Min(heavyCount, totalEnemies);

        int normalCount = totalEnemies - heavyCount;

        if (waveText != null)
            waveText.text = $"Wave {currentWave}";

        List<GameObject> spawnQueue = GenerateMixedSpawnList(normalEnemyPrefab, heavyEnemyPrefab, normalCount, heavyCount);

        foreach (GameObject prefab in spawnQueue)
        {
            SpawnEnemy(prefab);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        spawning = false;
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

        GameObject enemyGO = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Assign waypoints and speed for HeavyEnemyMovement
        var heavyMove = enemyGO.GetComponent<HeavyEnemyMovement>();
        if (heavyMove != null)
        {
            heavyMove.waypoints = waypoints;
            heavyMove.speed *= Mathf.Pow(1.05f, currentWave - 1);
            // ❌ removed: cannot assign baseTransform
        }

        // Assign waypoints and speed for MissileEnemyMovement
        var missileMove = enemyGO.GetComponent<MissileEnemyMovement>();
        if (missileMove != null)
        {
            missileMove.waypoints = waypoints;
            missileMove.speed *= Mathf.Pow(1.05f, currentWave - 1);
            // ❌ removed: cannot assign baseTransform
        }

        // Assign health scaling
        EnemyHealth hp = enemyGO.GetComponent<EnemyHealth>();
        if (hp != null)
            hp.maxHealth *= Mathf.Pow(difficultyMultiplier, currentWave - 1);
    }

    int EnemiesAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void UpdateUI()
    {
        if (countdownText != null && !spawning && EnemiesAlive() == 0)
            countdownText.text = $"Next wave in: {countdown:0.0}s";
        else if (countdownText != null)
            countdownText.text = "";

        if (waveText != null)
            waveText.text = $"Wave: {currentWave}";

        if (enemiesAliveText != null)
            enemiesAliveText.text = $"Enemies: {EnemiesAlive()}";
    }
}
