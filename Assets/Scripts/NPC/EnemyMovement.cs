using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float currentSpeed;
    private float baseY;
    private Coroutine slowCoroutine;

    [Header("Attack Settings")]
    public bool isRanged = false; // ✅ toggle between melee or ranged enemies
    public float attackRange = 8f;
    public float attackDamage = 10f;
    public float attackRate = 1f;
    public GameObject projectilePrefab; // ✅ for ranged attacks
    public Transform firePoint; // where projectiles spawn

    private bool reachedBase = false;
    private bool isAttacking = false;

    private BaseHealth baseHealth;
    private EnemyHealth enemyHealth;

    void Start()
    {
        currentSpeed = speed;
        baseY = transform.position.y;

        enemyHealth = GetComponent<EnemyHealth>();
        baseHealth = FindFirstObjectByType<BaseHealth>();

        // Ignore collisions between enemies
        Collider thisCollider = GetComponent<Collider>();
        Collider[] allEnemies = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        foreach (Collider c in allEnemies)
        {
            if (c != thisCollider && c.CompareTag("Enemy"))
                Physics.IgnoreCollision(thisCollider, c);
        }

        if (waypoints == null || waypoints.Length == 0)
            Debug.LogError($"{name} has no waypoints assigned!");
    }

    void Update()
    {
        if (reachedBase)
        {
            if (!isAttacking && baseHealth != null && baseHealth.currentHealth > 0)
            {
                float dist = Vector3.Distance(transform.position, baseHealth.transform.position);
                if (dist <= attackRange)
                    StartCoroutine(AttackBase());
            }
            return;
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 targetPos = new Vector3(target.position.x, baseY, target.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * currentSpeed * Time.deltaTime;

        // Rotate toward direction
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        float distXZ = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(target.position.x, 0, target.position.z)
        );

        if (distXZ < 0.2f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                reachedBase = true;
                currentSpeed = 0f;
            }
        }
    }

    private IEnumerator AttackBase()
    {
        isAttacking = true;

        while (enemyHealth != null && enemyHealth.currentHealth > 0 &&
               baseHealth != null && baseHealth.currentHealth > 0)
        {
            if (isRanged && projectilePrefab != null && firePoint != null)
            {
                ShootProjectile();
            }
            else
            {
                baseHealth.TakeDamage(attackDamage);
            }

            yield return new WaitForSeconds(1f / attackRate);
        }

        isAttacking = false;
    }

    void ShootProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (baseHealth.transform.position - firePoint.position).normalized;
        proj.transform.rotation = Quaternion.LookRotation(dir);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float projectileSpeed = 15f;
            rb.linearVelocity = dir * projectileSpeed;
        }

        // If projectile has its own script, pass baseHealth
        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.SetTarget(baseHealth.transform);
            projScript.damage = attackDamage;
        }
    }

    public void ApplySlow(float amount, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(amount, duration));
    }

    private IEnumerator SlowRoutine(float amount, float duration)
    {
        currentSpeed = speed * (1f - amount);
        yield return new WaitForSeconds(duration);
        currentSpeed = speed;
        slowCoroutine = null;
    }
}
