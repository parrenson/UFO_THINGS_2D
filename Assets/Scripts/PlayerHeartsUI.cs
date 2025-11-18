using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartsUI : MonoBehaviour
{
    [Header("Referencias")]
    public HealthSystem_Player playerHealth;

    [Header("Corazones en pantalla (en orden)")]
    public Image[] heartImages;          // 3 imágenes de corazón
    public Sprite fullHeartSprite;       // sprite de corazón lleno
    public Sprite emptyHeartSprite;      // sprite de corazón vacío

    private void Start()
    {
        // Si no asignaste el player en el inspector, lo busca por tag "Player"
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<HealthSystem_Player>();
            }
        }

        UpdateHearts();
    }

    private void Update()
    {
        if (playerHealth == null || heartImages == null) return;
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        int current = playerHealth.currentHealth;
        int max = playerHealth.maxHealth;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            if (i < current)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }

            // Por si algún día maxHealth < cantidad de corazones
            heartImages[i].enabled = (i < max);
        }
    }
}