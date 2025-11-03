using UnityEngine;
using System.Collections.Generic;

public class TeslaController : MonoBehaviour
{
    [Header("Fire Point")]
    public Transform firePoint; // Top of the tower where beams originate

    [Header("Targeting")]
    public string enemyTag = "Enemy";
    public float range = 15f;
    public int maxTargets = 3;

    [Header("Damage & Slow")]
    public float damagePerSecond = 5f;
    [Range(0f, 1f)] public float slowAmount = 0.5f; // 50% slow
    public float slowDuration = 1f;

    [Header("Visuals")]
    public LineRenderer lightningBeamPrefab;
    public float beamWidth = 0.2f;
    public Color beamColor = Color.cyan;

    private List<Transform> targets = new List<Transform>();
    private List<LineRenderer> beams = new List<LineRenderer>();

    void Start()
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint not assigned on TeslaController.");
        }

        // Pre-instantiate LineRenderers
        for (int i = 0; i < maxTargets; i++)
        {
            if (lightningBeamPrefab != null)
            {
                LineRenderer beam = Instantiate(lightningBeamPrefab, firePoint.position, Quaternion.identity);
                beam.positionCount = 2;
                beam.startWidth = beamWidth;
                beam.endWidth = beamWidth;
                beam.material = new Material(Shader.Find("Unlit/Color"));
                beam.material.color = beamColor;
                beam.enabled = false;
                beams.Add(beam);
            }
        }
    }

    void Update()
    {
        FindTargets();
        UpdateBeams();
    }

    void FindTargets()
    {
        targets.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= range && targets.Count < maxTargets)
            {
                targets.Add(e.transform);
            }
        }
    }

    void UpdateBeams()
    {
        for (int i = 0; i < beams.Count; i++)
        {
            LineRenderer beam = beams[i];

            if (i < targets.Count && targets[i] != null)
            {
                Transform t = targets[i];

                // Apply damage
                EnemyHealth enemy = t.GetComponent<EnemyHealth>();
                if (enemy != null)
                    enemy.TakeDamage(damagePerSecond * Time.deltaTime);

                // Apply slow
                EnemyMovement movement = t.GetComponent<EnemyMovement>();
                if (movement != null)
                    movement.ApplySlow(slowAmount, slowDuration);

                // Set beam positions
                beam.SetPosition(0, firePoint.position); // beam starts from tower top
                beam.SetPosition(1, t.position);
                beam.enabled = true;
            }
            else
            {
                beam.enabled = false; // hide unused beams
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
