using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CartaGlow : MonoBehaviour
{
    public Light2D cartaLight;
    public float pulseSpeed = 2f;
    public float minIntensity = 0.3f;
    public float maxIntensity = 0.7f;

    void Update()
    {
        if (cartaLight != null)
        {
            cartaLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        }
    }
}
