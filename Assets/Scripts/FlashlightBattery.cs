using UnityEngine;
using UnityEngine.UI; // Necesario si muestras la batería como texto o barra
using UnityEngine.Rendering.Universal;

public class FlashlightBattery : MonoBehaviour
{
    public Light2D flashlight;          // Asigna la luz
    public Text batteryText;            // Referencia al texto UI
    public float maxBattery = 100f;     // Batería máxima
    public float battery;               // Batería actual
    public float drainRate = 10f;       // Consumo por segundo
    public KeyCode switchKey = KeyCode.F; // Tecla para encender/apagar
    private bool isOn = false;

    void Start()
    {
        battery = maxBattery;
        flashlight.enabled = false;
    }

    void Update()
    {
        // Encender/apagar la linterna
        if (Input.GetKeyDown(switchKey) && battery > 0)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }

        // Consumir batería si está encendida
        if (isOn && battery > 0)
        {
            battery -= drainRate * Time.deltaTime;
            if (battery <= 0.01f)
            {
                battery = 0f;
                isOn = false;
                flashlight.enabled = false;
            }
        }

        // Actualiza la UI
        if (batteryText)
        {
            batteryText.text = $"Batería: {(int)battery}";
        }
    }

    // Método para recargar batería
    public void Recharge(float amount)
    {
        battery = Mathf.Clamp(battery + amount, 0, maxBattery);
    }
}
