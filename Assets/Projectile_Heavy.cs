using UnityEngine;

public class Projectile_Heavy : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 25f;
    public float lifetime = 5f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Raycast to detect collision
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distanceThisFrame + 0.1f))
        {
            // Only check for BaseHealth
            BaseHealth baseHealth = hit.collider.GetComponent<BaseHealth>();
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Move projectile
        transform.position += dir * distanceThisFrame;
        transform.LookAt(target);
    }
}
