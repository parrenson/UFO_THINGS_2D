using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int cartasRecolectadas = 0;
    public int cartasNecesarias = 3;

    public GameObject cartaPanel;
    public TMP_Text mensajeCartaText;

    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RecolectarCarta(string mensaje)
    {
        cartasRecolectadas++;
        Inventario inventario = FindFirstObjectByType<Inventario>();
        if (inventario != null)
        {
            inventario.cartasRecolectadas = cartasRecolectadas;
            inventario.ActualizarInventario();
        }
        MostrarCarta(mensaje);
        VerificarVictoria();
    }

    public void MostrarCarta(string mensaje)
    {
        cartaPanel.SetActive(true);
        mensajeCartaText.text = mensaje;
        if (sonidoAbrir != null)
            AudioSource.PlayClipAtPoint(sonidoAbrir, Camera.main.transform.position);
    }

    public void CerrarCartaPanel()
    {
        cartaPanel.SetActive(false);
        if (sonidoCerrar != null)
            AudioSource.PlayClipAtPoint(sonidoCerrar, Camera.main.transform.position);
    }

    void VerificarVictoria()
    {
        if (cartasRecolectadas >= cartasNecesarias)
        {
            Debug.Log("¡Has recolectado todas las cartas! ¡Ganaste!");
        }
    }
}
