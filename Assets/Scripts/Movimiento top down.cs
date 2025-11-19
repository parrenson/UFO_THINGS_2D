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
    private Vector2 lastMoveDir = Vector2.down;

    [Header("Sonido de pasos")]
    public AudioClip footstepClip;
    [Range(0f, 1f)] public float footstepVolume = 0.3f;
    private AudioSource stepAudioSource;

    [Header("Linterna")]
    public Transform flashlightPivot;
    public float flashlightAngleOffset = -90f;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        stepAudioSource = gameObject.AddComponent<AudioSource>();
        stepAudioSource.clip = footstepClip;
        stepAudioSource.loop = true;
        stepAudioSource.playOnAwake = false;
        stepAudioSource.spatialBlend = 0f;
        stepAudioSource.volume = footstepVolume;
        stepAudioSource.priority = 256;
    }

    private void Update()
    {
        direccion = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        bool estaCaminando = direccion.sqrMagnitude > 0.01f;

        if (estaCaminando)
            lastMoveDir = direccion;

        if (animator != null)
        {
            animator.SetBool("isMoving", estaCaminando);
            Vector2 dirAnim = estaCaminando ? direccion : lastMoveDir;
            animator.SetFloat("moveX", dirAnim.x);
            animator.SetFloat("moveY", dirAnim.y);
        }

        if (estaCaminando && !stepAudioSource.isPlaying && footstepClip != null)
        {
            stepAudioSource.Play();
        }
        else if (!estaCaminando && stepAudioSource.isPlaying)
        {
            stepAudioSource.Stop();
        }

        ActualizarLinterna();
    }

    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.deltaTime);
    }

    private void ActualizarLinterna()
    {
        if (flashlightPivot == null) return;
        if (lastMoveDir.sqrMagnitude < 0.01f) return;
        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg + flashlightAngleOffset;
        flashlightPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
