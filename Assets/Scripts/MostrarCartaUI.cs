using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MostrarCartaUI : MonoBehaviour
{
    public GameObject panelCarta;      // El panel que muestra la carta
    public TMP_Text textoCarta;            // El componente Text dentro del panel

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void MostrarCarta(string mensaje)
    {
        textoCarta.text = mensaje;
        panelCarta.SetActive(true);    // Muestra el panel
    }

    public void OcultarCarta()
    {
        panelCarta.SetActive(false);   // Oculta el panel (puedes llamar esto al cerrar)
    }
}
