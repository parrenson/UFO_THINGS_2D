using UnityEngine;

public class BatteryCharger : MonoBehaviour
{
    public float chargeRate = 20f;
    public AudioClip chargeClip;
    public AudioClip electricAmbientClip;
    public float soundInterval = 2f;

    private FlashlightBattery playerBattery;
    private bool playerInZone = false;
    private bool isCharging = false;
    public KeyCode chargeKey = KeyCode.R;
    private AudioSource chargeSound;
    private AudioSource ambientSound;
    private float nextSoundTime;

    void Start()
    {
        chargeSound = gameObject.AddComponent<AudioSource>();
        chargeSound.playOnAwake = false;
        chargeSound.loop = false;
        chargeSound.clip = chargeClip;

        ambientSound = gameObject.AddComponent<AudioSource>();
        ambientSound.playOnAwake = false;
        ambientSound.loop = true;
        ambientSound.clip = electricAmbientClip;
    }

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
            StopChargingSounds();
        }
    }

    void Update()
    {
        if (playerInZone && playerBattery != null)
        {
            if (Input.GetKeyDown(chargeKey))
            {
                isCharging = !isCharging;

                if (isCharging)
                    StartChargingSounds();
                else
                    StopChargingSounds();
            }
        }
        else if (isCharging)
        {
            isCharging = false;
            StopChargingSounds();
        }

        if (isCharging && playerBattery != null && playerBattery.battery < playerBattery.maxBattery)
        {
            playerBattery.battery += chargeRate * Time.deltaTime;
            playerBattery.battery = Mathf.Min(playerBattery.battery, playerBattery.maxBattery);

            if (playerBattery.battery >= playerBattery.maxBattery)
            {
                isCharging = false;
                StopChargingSounds();
            }

            if (Time.time >= nextSoundTime)
            {
                PlayChargingPulse();
                nextSoundTime = Time.time + chargeClip.length + soundInterval;
            }
        }
    }

    void StartChargingSounds()
    {
        nextSoundTime = Time.time;
        PlayChargingPulse();

        
    }

    void StopChargingSounds()
    {
        if (chargeSound != null)
            chargeSound.Stop();


    }

    void PlayChargingPulse()
    {
        if (chargeSound != null && chargeClip != null)
            chargeSound.PlayOneShot(chargeClip);
    }
}
