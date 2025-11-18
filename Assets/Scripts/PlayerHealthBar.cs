using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public HealthSystem_Player playerHealth;
    public Slider slider;

    private void Start()
    {
        // Si no asignaste el Slider en el inspector, intenta coger el del mismo objeto
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        // Si no asignaste el player en el inspector, lo busca por tag "Player"
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<HealthSystem_Player>();
            }
        }

        if (playerHealth != null && slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;
        }
    }

    private void Update()
    {
        if (playerHealth == null || slider == null) return;

        // Actualiza la barra según la vida actual
        slider.value = playerHealth.currentHealth;
    }
}