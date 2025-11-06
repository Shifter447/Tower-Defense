using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyHealth))]
public class HeavyEnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float baseY;
    private float currentSpeed;

    public float attackRange = 8f;
    public float attackDamage = 20f;
    public float attackRate = 1.5f;
    public GameObject projectilePrefab;
    public Transform[] firePoints;

    private bool reachedBase = false;
    private bool isAttacking = false;
    private EnemyHealth enemyHealth;
    private BaseHealth baseHealth;

    void Start()
    {
        currentSpeed = speed;
        baseY = transform.position.y;
        enemyHealth = GetComponent<EnemyHealth>();
        baseHealth = FindFirstObjectByType<BaseHealth>();
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
            foreach (Transform firePoint in firePoints)
            {
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Projectile_Heavy projScript = proj.GetComponent<Projectile_Heavy>();
                if (projScript != null)
                {
                    projScript.damage = attackDamage;
                    projScript.SetTarget(baseHealth.transform);
                }
            }

            yield return new WaitForSeconds(1f / attackRate);
        }

        isAttacking = false;
    }
}
