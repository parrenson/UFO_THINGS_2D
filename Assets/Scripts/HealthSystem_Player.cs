using System.Collections;
using UnityEngine;

public class HealthSystem_Player : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Corazones")]
    public UnityEngine.UI.Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Animator animator;
    private Movimientotopdown movementScript;
    private Collider2D col;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        movementScript = GetComponent<Movimientotopdown>();
        col = GetComponent<Collider2D>();

        UpdateHeartsUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            hearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Desactivar movimiento y colisión
        if (movementScript != null) movementScript.enabled = false;
        if (col != null) col.enabled = false;

        // Lanzar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("deathTrigger");
        }

        // Iniciar secuencia de muerte (esperar animación)
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Esperamos un frame para que el Animator cambie al estado Death
        yield return null;

        float waitTime = 1.0f; // tiempo por defecto

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Si estamos en el estado Death, usamos su duración
            if (stateInfo.IsName("Death"))
            {
                waitTime = stateInfo.length;
            }
        }

        // Esperar duración de la animación
        yield return new WaitForSeconds(waitTime);

        // Mostrar Game Over
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager.Instance es null, ¿falta el GameOverManager en la escena?");
        }
    }
}