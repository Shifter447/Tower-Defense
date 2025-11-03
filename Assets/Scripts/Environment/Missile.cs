using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float speed = 15f;         // how fast it travels (affects travelTime)
    public float arcHeight = 5f;      // peak of arc
    public float damage = 25f;
    public float lifetime = 6f;

    [Tooltip("Euler offset to match your model's forward axis (e.g. 0,180,0 if model faces -Z).")]
    public Vector3 rotationEulerOffset = Vector3.zero;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float distance;
    private float travelTime;
    private float elapsed;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        startPos = transform.position;

        targetPos = (target != null) ? target.position : startPos + transform.forward * 10f;
        distance = Vector3.Distance(startPos, targetPos);
        travelTime = Mathf.Max(0.001f, distance / Mathf.Max(0.0001f, speed));
        elapsed = 0f;

        // initial facing toward target (plus offset)
        Vector3 initialDir = (targetPos - startPos).normalized;
        if (initialDir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(initialDir) * Quaternion.Euler(rotationEulerOffset);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
            return;

        // keep updating target pos in case base moves
        targetPos = target.position;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);

        // compute current and next positions along the arc
        Vector3 currentPos = GetArcPosition(t);
        float dt = Time.deltaTime / travelTime;
        float nextT = Mathf.Clamp01(t + dt);
        Vector3 nextPos = GetArcPosition(nextT);

        // move first, then compute motion direction for rotation
        transform.position = currentPos;

        // tangent / motion direction
        Vector3 motionDir = nextPos - currentPos;

        if (motionDir.sqrMagnitude > 0.000001f)
        {
            Quaternion desired = Quaternion.LookRotation(motionDir.normalized);
            // apply model-local euler offset
            desired *= Quaternion.Euler(rotationEulerOffset);

            // smooth rotation to avoid jitter; you can increase LERP speed if needed
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, Mathf.Clamp01(20f * Time.deltaTime));
        }

        if (t >= 1f)
            HitTarget();
    }

    // Returns a point along the parabolic arc (linear XZ + vertical offset)
    private Vector3 GetArcPosition(float t)
    {
        // linear interp between start and target
        Vector3 linear = Vector3.Lerp(startPos, targetPos, t);

        // vertical arc: sin(pi * t) is smooth and peaks at t=0.5
        float height = arcHeight * Mathf.Sin(Mathf.PI * t);

        // add vertical offset (note: if your target y differs, this blends start->target y)
        // optional: use Mathf.Lerp(startPos.y, targetPos.y, t) + height if you want to respect target Y
        Vector3 pos = new Vector3(linear.x, Mathf.Lerp(startPos.y, targetPos.y, t) + height, linear.z);
        return pos;
    }

    private void HitTarget()
    {
        if (target != null)
        {
            BaseHealth b = target.GetComponent<BaseHealth>();
            if (b != null) b.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
