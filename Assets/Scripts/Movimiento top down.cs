using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientotopdown : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private Vector2 direccion;

    private Rigidbody2D rb2D;

    [Header("Sonido de pasos")]
    public AudioClip footstepClip;
    private AudioSource audioSource;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = footstepClip;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // 🔥 Muy importante: 0 = sonido 2D, no interfiere con otros
        audioSource.volume = 0.6f; // volumen moderado pa’ no tapar el ambiente
        audioSource.priority = 256; // 🔉 menor prioridad que el ambiente
    }


    private void Update()
    {
        direccion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool estaCaminando = direccion.magnitude > 0;

        if (estaCaminando && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!estaCaminando && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.deltaTime);
    }
}
