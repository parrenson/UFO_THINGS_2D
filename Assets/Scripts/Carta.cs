using UnityEngine;

public class Carta : MonoBehaviour
{
    public string mensajeCarta; // Texto que muestra la carta al recogerla

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.RecolectarCarta(mensajeCarta);
            Destroy(gameObject); // Se destruye la carta al recogerla
        }
    }
}
