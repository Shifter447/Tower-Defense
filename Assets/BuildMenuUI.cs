using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    public Button turretButton;
    public Button teslaButton;

    [HideInInspector] public BuildStand CurrentStand { get; private set; }
    public bool IsOpen => gameObject.activeSelf;

    private void Start()
    {
        // Initially hidden
        gameObject.SetActive(false);

        turretButton.onClick.AddListener(() => SelectTower("Turret"));
        teslaButton.onClick.AddListener(() => SelectTower("Tesla"));
    }

    public void Open(BuildStand stand)
    {
        CurrentStand = stand;
        gameObject.SetActive(true);

        // Position above the stand in screen space
        Vector3 worldPos = stand.transform.position + Vector3.up * 3f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
    }

    public void Close()
    {
        CurrentStand = null;
        gameObject.SetActive(false);
    }

    void SelectTower(string type)
    {
        if (CurrentStand == null) return;

        if (type == "Turret")
            BuildManager.Instance.BuildTurret(CurrentStand);
        else if (type == "Tesla")
            BuildManager.Instance.BuildTesla(CurrentStand);

        Close();
    }
}
