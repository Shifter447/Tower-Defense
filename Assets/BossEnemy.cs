using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyHealth))]
public class BossEnemy : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 5f;

    private int currentWaypoint = 0;
    private float baseY;
    private bool reachedBase = false;
    private bool isAttacking = false;

    public GameObject missilePrefab;
    public Transform[] missileFirePoints;
    public GameObject heavyProjectilePrefab;
    public Transform[] heavyFirePoints;
    public float missileAttackRate = 1f;
    public float heavyAttackRate = 1.5f;
    public float missileDamage = 10f;
    public float heavyDamage = 20f;

    private EnemyHealth enemyHealth;
    private BaseHealth baseHealth;

    void Start()
    {
        baseY = transform.position.y;
        enemyHealth = GetComponent<EnemyHealth>();
        baseHealth = FindFirstObjectByType<BaseHealth>();

        // Auto-assign fire points if empty
        if (missileFirePoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            foreach (Transform child in transform)
                if (child.name.Contains("MissileFirePoint")) points.Add(child);
            missileFirePoints = points.ToArray();
        }

        if (heavyFirePoints.Length == 0)
        {
            List<Transform> points = new List<Transform>();
            foreach (Transform child in transform)
                if (child.name.Contains("HeavyFirePoint")) points.Add(child);
            heavyFirePoints = points.ToArray();
        }
    }

    void Update()
    {
        if (!reachedBase)
            MoveAlongPath();
        else
        {
            if (!isAttacking)
                StartCoroutine(AttackBase());
        }
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
                reachedBase = true;
        }
    }

    private IEnumerator AttackBase()
    {
        isAttacking = true;

        while (enemyHealth != null && enemyHealth.currentHealth > 0 &&
               baseHealth != null && baseHealth.currentHealth > 0)
        {
            // Fire missile projectiles sequentially
            foreach (Transform fp in missileFirePoints)
            {
                GameObject m = Instantiate(missilePrefab, fp.position, fp.rotation);
                Missile missile = m.GetComponent<Missile>();
                if (missile != null)
                {
                    missile.SetTarget(baseHealth.transform);
                    missile.damage = missileDamage;
                }

                yield return new WaitForSeconds(missileAttackRate / missileFirePoints.Length);
            }

            // Fire heavy projectiles sequentially
            foreach (Transform fp in heavyFirePoints)
            {
                GameObject p = Instantiate(heavyProjectilePrefab, fp.position, fp.rotation);
                Projectile_Heavy proj = p.GetComponent<Projectile_Heavy>();
                if (proj != null)
                {
                    proj.SetTarget(baseHealth.transform);
                    proj.damage = heavyDamage;
                }

                yield return new WaitForSeconds(heavyAttackRate / heavyFirePoints.Length);
            }
        }

        isAttacking = false;
    }
}