using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class FlashlightBattery : MonoBehaviour
{
    [Header("Componentes")]
    public Light2D flashlight;
    public Text batteryText;
    public AudioSource audioSource;
    private AudioSource alertSource;

    [Header("Batería")]
    public float maxBattery = 100f;
    public float battery;
    public float drainRate = 20f;
    public KeyCode switchKey = KeyCode.F;

    [Header("Sonidos")]
    public AudioClip toggleSound;
    public AudioClip lowBatterySound;

    [Header("Configuración de alerta")]
    public float lowBatteryThreshold = 20f;
    public float alertInterval = 3f;
    private float nextAlertTime = 0f;

    private bool isOn = false;

    void Start()
    {
        battery = maxBattery;
        flashlight.enabled = false;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        alertSource = gameObject.AddComponent<AudioSource>();
        alertSource.playOnAwake = false;
        alertSource.loop = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            // Siempre suena el toggle
            if (toggleSound != null)
                audioSource.PlayOneShot(toggleSound);

            // Solo cambia estado si hay batería
            if (battery > 0)
            {
                isOn = !isOn;
                flashlight.enabled = isOn;

                if (!isOn && alertSource.isPlaying)
                    alertSource.Stop();
            }
        }

        if (isOn && battery > 0)
        {
            battery -= drainRate * Time.deltaTime;

            if (battery <= lowBatteryThreshold && battery > 0)
            {
                if (Time.time >= nextAlertTime && lowBatterySound != null)
                {
                    if (!alertSource.isPlaying)
                        alertSource.PlayOneShot(lowBatterySound);

                    nextAlertTime = Time.time + alertInterval;
                }
            }

            if (battery <= 0.01f)
            {
                battery = 0f;
                isOn = false;
                flashlight.enabled = false;
                alertSource.Stop();
                if (toggleSound != null)
                    audioSource.PlayOneShot(toggleSound);
            }
        }

        if (batteryText)
            batteryText.text = $"Batería: {(int)battery}";
    }

    public void Recharge(float amount)
    {
        battery = Mathf.Clamp(battery + amount, 0, maxBattery);
    }
}
