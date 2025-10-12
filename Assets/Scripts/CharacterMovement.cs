using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Vector2 moveInput;

    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [Range(0, 0.3f)] [SerializeField] private float smoothed = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private bool right = true;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxJumps = 2;
    private int jumpCount = 0;
    private bool jumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && jumpCount < maxJumps)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        float move = moveInput.x * speed;
        Vector3 targetVelocity = new Vector2(move, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector3.SmoothDamp(rb2D.linearVelocity, targetVelocity, ref velocity, smoothed);

        if (move > 0 && !right)
            Turn();
        else if (move < 0 && right)
            Turn();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
            jumpCount = 0;

        if (jumpPressed)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpCount++;
        }
    }

    private void Turn()
    {
        right = !right;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
