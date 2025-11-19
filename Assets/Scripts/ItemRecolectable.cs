using UnityEngine;

public class ItemRecolectable : MonoBehaviour
{
    public string nombreItem; // Ej: "Bateria", "Curacion", "Linterna", "Carta"
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (nombreItem == "Carta")
            {
                Carta carta = GetComponent<Carta>();
                if (carta != null)
                {
                    carta.RecolectarCarta();
                }
            }
            else
            {
                Inventario inventario = FindFirstObjectByType<Inventario>();
                if (inventario != null)
                {
                    inventario.AgregarItem(nombreItem);
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
