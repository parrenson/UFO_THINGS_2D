using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;
    private Vector2 lastMovementDir = Vector2.down; // Por defecto mira abajo

    void Awake()
    {
        transform.localScale = new Vector3(2f, 2f, 1f); // tamaño adecuado
    }




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalizamos para evitar velocidad extra en diagonal
        if (movement != Vector2.zero)
        {
            movement.Normalize();
            lastMovementDir = movement;  // Guardamos dirección actual
        }

        // ⬅️ Estos valores controlan WALK (Blend Tree)
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        // ⬅️ Estos valores controlan IDLE (Blend Tree Idle)
        animator.SetFloat("LastHorizontal", lastMovementDir.x);
        animator.SetFloat("LastVertical", lastMovementDir.y);

        // Control de transición (Idle ↔ Walk)
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
