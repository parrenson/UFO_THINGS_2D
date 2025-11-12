using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LinternaController : MonoBehaviour
{
    public Light2D luzLinterna;
    public float duracionBateria = 10f; // segundos
    private float tiempoRestante;
    private bool encendida = false;

    void Start()
    {
        luzLinterna.enabled = false;
        tiempoRestante = duracionBateria;
    }

    void Update()
    {
        if (encendida)
        {
            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante <= 0)
            {
                Apagar();
            }
        }
    }

    public void ToggleLinterna()
    {
        if (encendida)
        {
            Apagar();
        }
        else if (tiempoRestante > 0)
        {
            Encender();
        }
    }

    public void Encender()
    {
        encendida = true;
        luzLinterna.enabled = true;
    }

    public void Apagar()
    {
        encendida = false;
        luzLinterna.enabled = false;
    }

    public void RecargarBateria()
    {
        tiempoRestante = duracionBateria;
    }
}
