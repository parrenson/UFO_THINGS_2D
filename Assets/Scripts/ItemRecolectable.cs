using UnityEngine;

public class ItemRecolectable : MonoBehaviour
{
    public string nombreItem; // Ej: "Bateria", "Curacion", "Linterna"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Buscar el inventario en la escena y agregar el ítem
            Inventario inventario = FindFirstObjectByType<Inventario>();
            if (inventario != null)
            {
                inventario.AgregarItem(nombreItem);
            }

            // Destruir el objeto del ítem para simular que fue recogido
            Destroy(gameObject);
        }
    }
}

