using UnityEngine;

public class TurretController : MonoBehaviour
{
    public enum Axis { Forward, Right, Back, Left }

    [Header("Parts (siblings under TurretRoot)")]
    public Transform stabilizingBase;
    public Transform rotationalPlatform;
    public Transform turretHead;

    [Header("Targeting")]
    public string enemyTag = "Enemy";
    public float range = 20f;

    [Header("Locking Behavior")]
    [Tooltip("Once a target is acquired, the turret keeps it until it dies or leaves range.")]
    public bool lockOnTarget = true;

    [Header("Rotation Speeds (deg/sec)")]
    public float platformRotationSpeed = 180f;
    public float headRotationSpeed = 180f;

    [Header("Forward Axis Fix")]
    public Axis platformForwardAxis = Axis.Right;
    public float extraPlatformYawOffset = 0f;

    [Header("Head offsets & limits")]
    public float headPitchOffset = 0f;
    public float headYawOffset = 0f;
    public float minPitch = -10f;
    public float maxPitch = 60f;

    [Header("Shooting")]
    [Tooltip("Left and right barrel fire points")]
    public Transform[] firePoints;
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    private float fireCooldown = 0f;
    private int currentBarrelIndex = 0;

    private Transform targetEnemy;

    public float headAxisYawOffset = 0f;

    void Update()
    {
        if (lockOnTarget)
        {
            if (!IsValidTarget(targetEnemy))
                AcquireNearestTarget();
        }
        else
        {
            AcquireNearestTarget();
        }

        if (targetEnemy != null)
        {
            AimAtTarget();

            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                ShootFromNextBarrel();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    bool IsValidTarget(Transform t)
    {
        if (t == null) return false;
        if (!t.gameObject.activeInHierarchy) return false;
        float d = Vector3.Distance(transform.position, t.position);
        return d <= range;
    }

    void AcquireNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float best = Mathf.Infinity;
        Transform bestT = null;

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= range && dist < best)
            {
                best = dist;
                bestT = e.transform;
            }
        }

        targetEnemy = bestT;
    }

    float AxisToYawOffset(Axis a)
    {
        switch (a)
        {
            case Axis.Forward: return 0f;
            case Axis.Right: return -90f;
            case Axis.Back: return 180f;
            case Axis.Left: return 90f;
            default: return 0f;
        }
    }

    void AimAtTarget()
    {
        if (rotationalPlatform == null || turretHead == null || targetEnemy == null)
            return;

        // ---- PLATFORM (Yaw only) ----
        Vector3 toTargetPlat = targetEnemy.position - rotationalPlatform.position;
        toTargetPlat.y = 0f;

        if (toTargetPlat.sqrMagnitude > 0.001f)
        {
            Quaternion desiredPlat = Quaternion.LookRotation(toTargetPlat.normalized, Vector3.up);
            float yawOffset = AxisToYawOffset(platformForwardAxis) + extraPlatformYawOffset;
            desiredPlat *= Quaternion.Euler(0f, yawOffset, 0f);

            rotationalPlatform.rotation = Quaternion.RotateTowards(
                rotationalPlatform.rotation,
                desiredPlat,
                platformRotationSpeed * Time.deltaTime
            );
        }

        // ---- HEAD (Pitch, match Yaw) ----
        Vector3 toTargetHead = targetEnemy.position - turretHead.position;
        float flatDist = new Vector2(toTargetHead.x, toTargetHead.z).magnitude;
        float desiredPitch = Mathf.Atan2(toTargetHead.y, flatDist) * Mathf.Rad2Deg;
        desiredPitch += headPitchOffset;
        desiredPitch = Mathf.Clamp(desiredPitch, minPitch, maxPitch);

        float platformYaw = rotationalPlatform.rotation.eulerAngles.y;
        float headYaw = platformYaw + headYawOffset;
        Quaternion desiredHeadRot = Quaternion.Euler(desiredPitch, headYaw, 0f);

        turretHead.rotation = Quaternion.RotateTowards(
            turretHead.rotation,
            desiredHeadRot,
            headRotationSpeed * Time.deltaTime
        );
    }

    void ShootFromNextBarrel()
    {
        if (projectilePrefab == null || firePoints == null || firePoints.Length == 0)
            return;

        Transform firePoint = firePoints[currentBarrelIndex];
        if (firePoint != null)
        {
            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = projGO.GetComponent<Projectile>();
            if (proj != null && targetEnemy != null)
                proj.SetTarget(targetEnemy);
        }

        currentBarrelIndex = (currentBarrelIndex + 1) % firePoints.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        if (rotationalPlatform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(rotationalPlatform.position, rotationalPlatform.forward * 2f);
        }
        if (turretHead != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(turretHead.position, turretHead.forward * 2f);
        }
        if (targetEnemy != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(turretHead.position, targetEnemy.position);
        }
    }
}