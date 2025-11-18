using UnityEngine;

public class ParanoiaEffect : MonoBehaviour
{
    public CinemachineShake cinemachineShake;
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 1.5f;

    public void TriggerShake(float? duration = null, float? intensity = null)
    {
        if (cinemachineShake != null)
        {
            float dur = duration ?? shakeDuration;
            float inten = intensity ?? shakeIntensity;
            cinemachineShake.Shake(inten, dur);
        }
    }
}
