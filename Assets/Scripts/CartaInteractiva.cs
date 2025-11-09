using UnityEngine;

public class CartaInteractiva : MonoBehaviour
{
    public GameObject cartaPanel; // Arrastra aquí el panel de texto

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            cartaPanel.SetActive(!cartaPanel.activeSelf);
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
            cartaPanel.SetActive(false);
        }
    }
}

