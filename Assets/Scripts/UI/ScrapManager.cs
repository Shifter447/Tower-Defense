using UnityEngine;
using TMPro;

public class ScrapManager : MonoBehaviour
{
    public static ScrapManager Instance;

    [Header("Scrap Settings")]
    public int startingScrap = 100;
    public TextMeshProUGUI scrapText;

    private int currentScrap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentScrap = startingScrap;
        UpdateUI();
    }

    public void AddScrap(int amount)
    {
        currentScrap += amount;
        UpdateUI();
    }

    public bool SpendScrap(int cost)
    {
        if (currentScrap < cost)
            return false;

        currentScrap -= cost;
        UpdateUI();
        return true;
    }

    public int GetScrap()
    {
        return currentScrap;
    }

    void UpdateUI()
    {
        if (scrapText != null)
            scrapText.text = $"Scrap: {currentScrap}";
    }
}
