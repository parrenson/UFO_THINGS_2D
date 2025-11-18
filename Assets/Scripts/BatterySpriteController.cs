using UnityEngine;
using UnityEngine.UI;

public class BatterySpriteController : MonoBehaviour
{
    public Image batteryImage; 
    public Sprite[] batterySprites; 
    public FlashlightBattery flashlightBattery; 

    void Update()
    {
        float percent = flashlightBattery.battery / flashlightBattery.maxBattery;

        int index;
        if (percent <= 0.01f) index = 0;                
        else if (percent <= 0.20f) index = 1;             
        else if (percent <= 0.40f) index = 2;          
        else if (percent <= 0.60f) index = 3;         
        else if (percent <= 0.80f) index = 4;             
        else index = 5;                                   

        batteryImage.sprite = batterySprites[index];
    }
}
