using UnityEngine;

public class Projectile : MonoBehaviour
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
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        transform.position += dir * distanceThisFrame;
        transform.LookAt(target);
    }
}
