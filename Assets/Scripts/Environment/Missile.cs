using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 15f;       // horizontal speed
    public float arcHeight = 5f;    // max height of the arc
    public float damage = 25f;
    public float lifetime = 5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float distance;
    private float travelTime;
    private float elapsed;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        startPos = transform.position;
        targetPos = target.position;
        distance = Vector3.Distance(startPos, targetPos);
        travelTime = distance / speed;
        elapsed = 0f;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);

        // Calculate horizontal position
        Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, t);

        // Vertical offset (parabola)
        float heightOffset = 4f * arcHeight * t * (1 - t);
        Vector3 nextPos = horizontalPos + Vector3.up * heightOffset;

        // Rotate missile to face direction of motion
        Vector3 motionDir = nextPos - transform.position;
        if (motionDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(motionDir);

        // Move missile
        transform.position = nextPos;

        // Check if missile reached target
        if (t >= 1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (target != null)
        {
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
