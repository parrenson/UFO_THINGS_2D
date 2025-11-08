using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image linternaIcono;
    public Image bateriaIcono;
    public Image curacionIcono;

    [Header("Estado del Inventario")]
    public bool tieneLinterna = false;
    public int baterias = 0;
    public int curaciones = 0;

    void Start()
    {
        ActualizarInventario();
    }

    public void AgregarItem(string item)
    {
        switch (item)
        {
            case "Linterna":
                tieneLinterna = true;
                break;
            case "Bateria":
                baterias++;
                break;
            case "Curacion":
                curaciones++;
                break;
        }
        ActualizarInventario();
    }

    void ActualizarInventario()
    {
        linternaIcono.color = tieneLinterna ? Color.white : new Color(1, 1, 1, 0.3f);
        bateriaIcono.color = baterias > 0 ? Color.white : new Color(1, 1, 1, 0.3f);
        curacionIcono.color = curaciones > 0 ? Color.white : new Color(1, 1, 1, 0.3f);
    }
}
