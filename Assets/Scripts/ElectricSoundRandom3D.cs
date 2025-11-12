using UnityEngine;
using System.Collections;

public class ElectricSoundRandom3D : MonoBehaviour
{
    [Header("🎧 Configuración del sonido")]
    public AudioClip electricClip;
    public Vector2 timeRange = new Vector2(3f, 10f); // tiempo aleatorio entre sonidos
    public Vector2 areaRange = new Vector2(-5f, 5f); // desplazamiento aleatorio en el mapa (x, y)
    public float volume = 0.5f; // volumen general
    public float playDuration = 10f; // duración del fragmento de sonido

    private void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            // Espera aleatoria antes de sonar
            yield return new WaitForSeconds(Random.Range(timeRange.x, timeRange.y));

            // Crear objeto temporal donde se emite el sonido
            GameObject tempAudio = new GameObject("TempElectricSound");
            tempAudio.transform.position = new Vector3(
                transform.position.x + Random.Range(areaRange.x, areaRange.y),
                transform.position.y + Random.Range(areaRange.x, areaRange.y),
                transform.position.z
            );

            // Configurar el AudioSource
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = electricClip;
            audioSource.spatialBlend = 1f; // lo vuelve 3D
            audioSource.volume = volume;
            audioSource.pitch = Random.Range(0.9f, 1.1f); // variación leve para hacerlo más orgánico
            audioSource.Play();

            // Reproduce solo una parte del clip
            yield return new WaitForSeconds(playDuration);
            audioSource.Stop();

            Destroy(tempAudio, 0.5f);
        }
    }
}
