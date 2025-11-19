using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientotopdown : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private Vector2 direccion;

    private Vector2 direccionMirada = Vector2.up;

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
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.6f;
        audioSource.priority = 256;
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool estaCaminando = input.magnitude > 0;

        if (estaCaminando && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!estaCaminando && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (input.magnitude > 0)
        {
            direccion = input;
            direccionMirada = input;
        }
        else
        {
            direccion = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.deltaTime);

        if (direccionMirada != Vector2.zero)
        {
            float angle = Mathf.Atan2(direccionMirada.y, direccionMirada.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

}
