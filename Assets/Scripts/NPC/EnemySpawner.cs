using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public Transform[] waypoints;
    public float spawnInterval = 2f;  // How often basic enemies spawn

    [Header("Enemy Prefabs")]
    public GameObject Enemy;     // Your basic enemy prefab
    public GameObject Enemy_2;   // Your stronger, slower enemy prefab

    [Header("Enemy 2 Settings")]
    [Range(0f, 1f)] public float enemy2SpawnChance = 0.2f; // 20% chance for Enemy_2
    public float enemy2SpawnDelayMultiplier = 2f; // Makes next spawn take longer if Enemy_2 spawned

    private float nextSpawnTime;

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
        float rand = Random.value;

        // Pick which enemy to spawn
        if (rand <= enemy2SpawnChance)
        {
            prefabToSpawn = Enemy_2;
            nextSpawnTime = Time.time + spawnInterval * enemy2SpawnDelayMultiplier;
        }
        else
        {
            prefabToSpawn = Enemy;
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Spawn enemy
        GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Give waypoints to the enemy’s movement script
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
