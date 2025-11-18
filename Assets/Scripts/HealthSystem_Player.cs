using UnityEngine;

public class HealthSystem_Player : MonoBehaviour
{
    [Header("Vida del jugador")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Debug muerte")]
    public bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player recibe daño. Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player se curó. Vida actual: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("⚠️ EL PLAYER HA MUERTO ⚠️");

        // Aquí puedes luego:
        // - activar animación de muerte
        // - recargar escena
        // - mostrar pantalla de Game Over
    }
}
