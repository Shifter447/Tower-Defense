using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Tower Prefabs")]
    public GameObject turretPrefab;
    public GameObject teslaPrefab;

    [Header("Tower Costs")]
    public int turretCost = 25;
    public int teslaCost = 50;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void BuildTurret(BuildStand stand)
    {
        if (!ScrapManager.Instance.SpendScrap(turretCost))
        {
            Debug.Log("Not enough scrap for Turret!");
            return;
        }

        Instantiate(turretPrefab, stand.transform.position + Vector3.up * 0.2f, Quaternion.identity);
        Debug.Log($"Built Turret on {stand.name}");
    }

    public void BuildTesla(BuildStand stand)
    {
        if (!ScrapManager.Instance.SpendScrap(teslaCost))
        {
            Debug.Log("Not enough scrap for Tesla!");
            return;
        }

        Instantiate(teslaPrefab, stand.transform.position + Vector3.up * 0.2f, Quaternion.Euler(-90f, 0f, 0f));
        Debug.Log($"Built Tesla Tower on {stand.name}");
    }
}
