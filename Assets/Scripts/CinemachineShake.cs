using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CinemachineShake : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    public float shakeDuration = 0.3f;
    public float shakeAmplitude = 1.5f;
    public float shakeFrequency = 2f;

    private float shakeTimer = 0f;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        if (cinemachineCamera == null)
            cinemachineCamera = GetComponent<CinemachineCamera>();
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }
    }

    public void Shake(float? amplitude = null, float? duration = null, float? frequency = null)
    {
        if (noise != null)
        {
            noise.AmplitudeGain = amplitude ?? shakeAmplitude;
            noise.FrequencyGain = frequency ?? shakeFrequency;
            shakeTimer = duration ?? shakeDuration;
            StopAllCoroutines();
            StartCoroutine(StopShake());
        }
    }

    private IEnumerator StopShake()
    {
        yield return new WaitForSeconds(shakeTimer);
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}
