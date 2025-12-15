using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 13f;

    [Header("Ground Check")]
    public Transform groundCheck;          // boþ obje (ayaðýn altýna koyacaðýz)
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;             // WASD / stick
    private bool jumpPressed;

    // Saldýrýlar için yön niyeti (8 yön) burada dursun:
    public Vector2 Aim => moveInput.sqrMagnitude > 0.01f ? moveInput.normalized : Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Jump'ý Update'te “yut”uyoruz, FixedUpdate'te uygularýz.
        if (jumpPressed)
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            jumpPressed = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Karakterin yönünü çevir (basit):
        if (moveInput.x > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ---- INPUT SYSTEM: PlayerInput (Send Messages) ile otomatik çaðrýlýr ----
    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();

    public void OnJump(InputValue v)
    {
        if (v.isPressed) jumpPressed = true;
    }

    public void OnLight(InputValue v)
    {
        if (!v.isPressed) return;
        Debug.Log($"{name} LIGHT | aim: {Aim}");
    }

    public void OnHeavy(InputValue v)
    {
        if (!v.isPressed) return;
        Debug.Log($"{name} HEAVY | aim: {Aim}");
    }

    public void OnSkill(InputValue v)
    {
        if (!v.isPressed) return;
        Debug.Log($"{name} SKILL | aim: {Aim}");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
