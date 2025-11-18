using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem_Player hp = collision.GetComponent<HealthSystem_Player>();
            if (hp != null)
            {
                hp.Heal(healAmount);
            }

            Destroy(gameObject); // elimina el item tras recogerlo
        }
    }
}