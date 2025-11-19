using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimientotopdown : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private Vector2 direccion;

    private Rigidbody2D rb2D;

    [Header("Animación")]
    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down; // donde quedó mirando la última vez

    [Header("Sonido de pasos")]
    public AudioClip footstepClip;
    private AudioSource audioSource;

    [Header("Linterna")]
    public Transform flashlightPivot;       // hijo que tiene la luz
    public float flashlightAngleOffset = -90f;
    // Ajusta este offset si la linterna apunta raro:
    //  - Si tu pivot apunta hacia arriba (eje Y) por defecto -> deja -90 o 0 según veas
    //  - Si apunta hacia la derecha (eje X), pon 0

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        // Animator del player
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Movimientotopdown: No se encontró Animator en el player.");
        }

        // AudioSource para pasos
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
        // 👉 Esto ya usa WASD y flechas por defecto (Input Manager de Unity)
        direccion = new Vector2(
            Input.GetAxisRaw("Horizontal"),   // A/D o flechas ← →
            Input.GetAxisRaw("Vertical")      // W/S o flechas ↑ ↓
        ).normalized;

        bool estaCaminando = direccion.sqrMagnitude > 0.01f;

        // Guardar última dirección para Idle y linterna
        if (estaCaminando)
        {
            lastMoveDir = direccion;
        }

        // --- ANIMACIÓN ---
        if (animator != null)
        {
            animator.SetBool("isMoving", estaCaminando);

            // Si se mueve, usamos direccion; si no, la última dirección mirada
            Vector2 dirAnim = estaCaminando ? direccion : lastMoveDir;

            animator.SetFloat("moveX", dirAnim.x);
            animator.SetFloat("moveY", dirAnim.y);
        }

        // --- SONIDO DE PASOS ---
        if (estaCaminando && !audioSource.isPlaying && footstepClip != null)
        {
            audioSource.Play();
        }
        else if (!estaCaminando && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // --- LINTERNAAAA 💡 ---
        ActualizarLinterna();
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.deltaTime);
    }

    private void ActualizarLinterna()
    {
        if (flashlightPivot == null) return;

        // Usamos la última dirección en la que miró el jugador
        if (lastMoveDir.sqrMagnitude < 0.01f)
            return; // si nunca se movió aún, no giramos

        // Dirección a ángulo
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg + flashlightAngleOffset;

        flashlightPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}