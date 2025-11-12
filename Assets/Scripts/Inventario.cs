using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventario : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image linternaIcono;
    public Image bateriaIcono;
    public Image curacionIcono;
    public TMP_Text Bateriatexto;
    public TMP_Text Curaciontexto;

    [Header("Estado del Inventario")]
    public bool tieneLinterna = false;
    public int baterias = 0;
    public int curaciones = 0;

    [Header("Referencias externas")]
    public PlayerController jugador;    // Asignar en Inspector

    void Start()
    {
        ActualizarInventario();
    }

    public void AgregarItem(string item)
    {
        switch (item)
        {
           
            case "Bateria":
                baterias++;
                break;
            case "Curacion":
                curaciones++;
                break;
        }
        ActualizarInventario();
    }


    public void UsarCuracion()
    {
        if (curaciones > 0)
        {
            curaciones--;
            jugador.Curar(20); // +20 HP por ejemplo
            ActualizarInventario();
        }
    }

    public bool UsarBateria()
    {
        if (baterias > 0)
        {
            baterias--;
            ActualizarInventario();
            return true;
        }
        return false;
    }

    void Update()
    {
        // Teclas rápidas de uso
        if (Input.GetKeyDown(KeyCode.H)) UsarCuracion();
        if (Input.GetKeyDown(KeyCode.R)) UsarBateria();
    }

    void ActualizarInventario()
    {
        //linternaIcono.color = tieneLinterna ? Color.white : new Color(1, 1, 1, 0.3f);
        bateriaIcono.color = baterias > 0 ? Color.white : new Color(1, 1, 1, 0.3f);
        curacionIcono.color = curaciones > 0 ? Color.white : new Color(1, 1, 1, 0.3f);

        Bateriatexto.text = "x" + baterias;
        Curaciontexto.text = "x" + curaciones;
    }
}