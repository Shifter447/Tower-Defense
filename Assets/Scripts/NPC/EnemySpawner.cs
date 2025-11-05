using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public float spawnInterval = 2f;

    [Header("Enemy Prefabs")]
    public GameObject Enemy;     // Basic enemy
    public GameObject Enemy_2;   // Heavy enemy

    [Header("Enemy 2 Settings")]
    [Range(0f, 1f)] public float maxEnemy2Ratio = 0.3f; // Max fraction of Enemy_2
    public float enemy2SpawnDelayMultiplier = 2f; // Delay if Enemy_2 spawns

    private float nextSpawnTime;
    private int totalSpawned = 0;
    private int enemy2Count = 0;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject prefabToSpawn;

        int enemy1Count = totalSpawned - enemy2Count;

        // Calculate chance to spawn Enemy_2
        float currentEnemy2Ratio = totalSpawned > 0 ? (float)enemy2Count / totalSpawned : 0f;

        if (currentEnemy2Ratio < maxEnemy2Ratio)
        {
            // Slightly favor Enemy_1 so it's always more
            float chance = Random.value;
            if (chance < 0.5f)
            {
                prefabToSpawn = Enemy_2;
                enemy2Count++;
                nextSpawnTime = Time.time + spawnInterval * enemy2SpawnDelayMultiplier;
            }
            else
            {
                prefabToSpawn = Enemy;
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
        else
        {
            // Spawn Enemy 1 to keep them in majority
            prefabToSpawn = Enemy;
            nextSpawnTime = Time.time + spawnInterval;
        }

        totalSpawned++;

        // Spawn enemy
        GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Assign waypoints
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.waypoints = waypoints;
        }
        else
        {
            Debug.LogWarning($"Spawned enemy '{enemy.name}' has no EnemyMovement script!");
        }
    }
}
