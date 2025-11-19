using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cartas")]
    public int cartasRecolectadas = 0;
    public int cartasNecesarias = 3;

    [Header("UI de la Carta")]
    public GameObject cartaPanel;
    public TMP_Text mensajeCartaText;

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

    void MostrarCarta(string mensaje)
    {
        cartaPanel.SetActive(true);
        mensajeCartaText.text = mensaje;
        Debug.Log("Mostrando carta: " + mensaje);

    }

    void VerificarVictoria()
    {
        if (cartasRecolectadas >= cartasNecesarias)
        {
            Debug.Log("¡Has recolectado todas las cartas! ¡Ganaste!");
            // Aquí puedes activar pantalla de victoria
        }
    }

    public void CerrarCartaPanel()
    {
        cartaPanel.SetActive(false);
    }

    void Update()
    {
        if (cartaPanel.activeSelf)  // Si el panel está abierto
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Detecta la tecla espacio
            {
                CerrarCartaPanel();
            }
        }
    }
}
