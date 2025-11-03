using UnityEngine;
using UnityEngine.UI;

public class BaseHealthBar : MonoBehaviour
{
    public BaseHealth baseHealth;
    public Slider slider;
    public Vector3 offset = new Vector3(0, 3f, 0);

    void LateUpdate()
    {
        if (baseHealth == null || slider == null)
            return;

        slider.maxValue = baseHealth.maxHealth;
        slider.value = baseHealth.currentHealth;

        transform.position = baseHealth.transform.position + offset;
        transform.LookAt(Camera.main.transform);
    }
}
