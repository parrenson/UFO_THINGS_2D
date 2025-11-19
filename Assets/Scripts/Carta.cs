using UnityEngine;

public class Carta : MonoBehaviour
{
    public string mensajeCarta;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject canvasNotas = GameObject.Find("CanvasNotas");
            if (canvasNotas != null)
            {
                Transform panelNota = canvasNotas.transform.Find("Nota");
                if (panelNota != null)
                {
                    panelNota.gameObject.SetActive(true);
                    MostrarCartaUI cartaUI = panelNota.GetComponent<MostrarCartaUI>();
                    if (cartaUI != null)
                        cartaUI.MostrarCarta(mensajeCarta);
                }
            }
            Destroy(gameObject);
        }
    }


}
