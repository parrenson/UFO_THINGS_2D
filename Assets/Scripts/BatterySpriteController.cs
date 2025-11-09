using UnityEngine;
using UnityEngine.UI;

public class BatterySpriteController : MonoBehaviour
{
    public Image batteryImage; // Arrastra tu Image aquí
    public Sprite[] batterySprites; // Drag todas las variantes en order: empty, red, orange, yellow, blue, green (según tu diseño)
    public FlashlightBattery flashlightBattery; // Asume que tienes este script en escena

    void Update()
    {
        float percent = flashlightBattery.battery / flashlightBattery.maxBattery;
        // Elige índice según porcentaje
        int index;
        if (percent <= 0.01f) index = 0;                  // Sin batería
        else if (percent <= 0.20f) index = 1;             // Rojo
        else if (percent <= 0.40f) index = 2;             // Naranja
        else if (percent <= 0.60f) index = 3;             // Amarillo
        else if (percent <= 0.80f) index = 4;             // Azul
        else index = 5;                                   // Verde

        batteryImage.sprite = batterySprites[index];
    }
}
