using UnityEngine;

public class BuildStand : MonoBehaviour
{
    private Renderer rend;
    private Color startColor;

    [Header("UI")]
    public BuildMenuUI buildMenu; // assign in inspector

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            startColor = rend.material.color;

        if (buildMenu == null)
            Debug.LogError($"BuildMenuUI not assigned on {name}!");
    }

    void OnMouseEnter()
    {
        if (rend != null)
            rend.material.color = Color.green; // highlight on hover
    }

    void OnMouseExit()
    {
        if (rend != null)
            rend.material.color = startColor;
    }

    void OnMouseDown()
    {
        if (buildMenu == null) return;

        // Toggle menu: if already open on this stand, close it
        if (buildMenu.IsOpen && buildMenu.CurrentStand == this)
        {
            buildMenu.Close();
            return;
        }

        // Open menu above this stand
        buildMenu.Open(this);
    }
}
