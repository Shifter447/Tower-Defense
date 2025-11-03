using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyHealth))]
public class MissileEnemyMovement : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float speed = 3f;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float baseY;

    [Header("Attack Settings")]
    public GameObject missilePrefab;
    public Transform firePoint;
    public float attackRate = 1f;
    public float attackDamage = 10f;

    [Header("Animation")]
    public Animator animator;
    public float shoulderAnimationDuration = 1f; // duration of default animation

    private bool reachedBase = false;
    private bool isAttacking = false;
    private EnemyHealth enemyHealth;
    private BaseHealth baseHealth;

    void Start()
    {
        baseY = transform.position.y;
        enemyHealth = GetComponent<EnemyHealth>();
        baseHealth = FindFirstObjectByType<BaseHealth>();
    }

    void Update()
    {
        if (!reachedBase)
            MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 targetPos = new Vector3(target.position.x, baseY, target.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

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
                StartCoroutine(WaitAndStartAttacking());
            }
        }
    }

    private IEnumerator WaitAndStartAttacking()
    {
        // Wait for the shoulder-opening animation to finish
        yield return new WaitForSeconds(shoulderAnimationDuration);

        // Start attacking
        StartCoroutine(AttackBase());
    }

    private IEnumerator AttackBase()
    {
        isAttacking = true;

        while (enemyHealth != null && enemyHealth.currentHealth > 0 &&
               baseHealth != null && baseHealth.currentHealth > 0)
        {
            if (missilePrefab != null && firePoint != null)
            {
                ShootMissile();
            }

            yield return new WaitForSeconds(1f / attackRate);
        }

        isAttacking = false;
    }

    private void ShootMissile()
    {
        GameObject missileGO = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
        Missile missileScript = missileGO.GetComponent<Missile>();
        if (missileScript != null)
        {
            missileScript.SetTarget(baseHealth.transform);
            missileScript.damage = attackDamage;
        }
    }
}
