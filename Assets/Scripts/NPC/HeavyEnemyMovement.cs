using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyHealth))]
public class HeavyEnemyMovement : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 4f;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float currentSpeed;
    private float baseY;
    private Coroutine slowCoroutine;

    [Header("Attack Settings")]
    public bool isRanged = true;
    public float attackRange = 10f;
    public float attackDamage = 15f;
    public float attackRate = 1f;
    public GameObject projectilePrefab;
    public Transform[] firePoints; // multi-barrel
    private int currentBarrel = 0;

    private bool reachedBase = false;
    private bool isAttacking = false;

    [HideInInspector] public BaseHealth baseHealth;
    public Transform baseTransform => baseHealth != null ? baseHealth.transform : null; // ✅ read-only

    private EnemyHealth enemyHealth;

    void Start()
    {
        currentSpeed = speed;
        baseY = transform.position.y;

        enemyHealth = GetComponent<EnemyHealth>();
        baseHealth = FindFirstObjectByType<BaseHealth>();

        // Ignore collisions with other enemies
        Collider thisCollider = GetComponent<Collider>();
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in allEnemies)
        {
            if (obj == this.gameObject) continue;
            Collider otherCollider = obj.GetComponent<Collider>();
            if (otherCollider != null)
                Physics.IgnoreCollision(thisCollider, otherCollider);
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
            if (isRanged && projectilePrefab != null && firePoints.Length > 0)
            {
                Transform firePoint = firePoints[currentBarrel];
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                Projectile projScript = proj.GetComponent<Projectile>();
                if (projScript != null)
                {
                    projScript.SetTarget(baseHealth.transform);
                    projScript.damage = attackDamage;
                }

                currentBarrel = (currentBarrel + 1) % firePoints.Length;
            }
            else
            {
                baseHealth.TakeDamage(attackDamage);
            }

            yield return new WaitForSeconds(1f / attackRate);
        }

        isAttacking = false;
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
