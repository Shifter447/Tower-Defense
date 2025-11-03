using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 500f;
    public float currentHealth;

    [Header("UI (optional)")]
    public Slider healthSlider;

    [Header("Game Over Settings")]
    public bool destroyOnDeath = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💥 Base destroyed!");

        if (destroyOnDeath)
            Destroy(gameObject);

        // TODO: You can trigger a GameOver screen or end condition here
        // Example: GameManager.Instance.GameOver();
    }
}
