using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Scrap Reward")]
    public int scrapValue = 10; // hoeveel scrap je krijgt als deze enemy doodgaat

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Geef scrap aan speler
        if (ScrapManager.Instance != null)
            ScrapManager.Instance.AddScrap(scrapValue);

        // Destroy enemy
        Destroy(gameObject);
    }
}
