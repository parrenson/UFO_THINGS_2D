using UnityEngine;

public class BatteryCharger : MonoBehaviour
{
    public float chargeRate = 20f;
    private FlashlightBattery playerBattery;
    private bool playerInZone = false;
    private bool isCharging = false;
    public KeyCode chargeKey = KeyCode.E;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerBattery = other.GetComponent<FlashlightBattery>();
            playerInZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            isCharging = false;
        }
    }

    void Update()
    {
        if (playerInZone && playerBattery != null)
        {
            if (Input.GetKeyDown(chargeKey))
            {
                isCharging = !isCharging;
            }
        }
        else
        {
            isCharging = false;
            isCharging = false;
        }

        if (isCharging && playerBattery != null && playerBattery.battery < playerBattery.maxBattery)
        {
            playerBattery.battery += chargeRate * Time.deltaTime;
            playerBattery.battery = Mathf.Min(playerBattery.battery, playerBattery.maxBattery);
            if (playerBattery.battery >= playerBattery.maxBattery)
            {
                isCharging = false;
            }
        }
    }
}
