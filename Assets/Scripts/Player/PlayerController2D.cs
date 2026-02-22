using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Visual (ONLY this flips)")]
    public Transform visual;
    public bool invertVisualFacing; // Ekledik: Eger karakterin cizimi sola bakiyorsa tickle

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 13f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("Facing Lock")]
    public bool facingLock;
    public int lockedFacing = 1;

    [Header("Jump")]
    public int maxJumps = 2;
    int jumpsUsed = 0;

    Rigidbody2D rb;
    Vector2 moveInput;
    bool jumpPressed;
    public bool controlLocked;

    int currentFacing = 1;

    public int Facing => facingLock ? lockedFacing : currentFacing;

    public Vector2 Aim
    {
        get
        {
            if (moveInput.sqrMagnitude > 0.01f) return moveInput.normalized;
            return (Facing == 1) ? Vector2.right : Vector2.left;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // ROOT ASLA FLIPLENMEZ
        transform.localScale = Vector3.one;

        // Visual'ı otomatik bul (Inspector'a bağlamayı unutursan diye)
        if (visual == null)
        {
            var t = transform.Find("Visual");
            if (t != null) visual = t;
        }

        // Hâlâ yoksa: root'u flipleyip her şeyi bozmayalım, dummy yarat
        if (visual == null)
        {
            var go = new GameObject("Visual");
            go.transform.SetParent(transform, false);
            visual = go.transform;
        }
    }

    void Update()
    {
        // Yerdeyken ve asagi dogru dusmuyorken (zaten ziplamisken tekrar yerdeymis gibi algilamasini onlemek icin)
        if (IsGrounded() && rb.linearVelocity.y <= 0.1f) 
        {
            jumpsUsed = 0;
        }

        if (jumpPressed && !controlLocked)
        {
            if (jumpsUsed < maxJumps)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Dikey hizi sifirla (double jump icin lazim)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpsUsed++;
            }
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        // Her ihtimale karşı root scale'i temiz tut
        if (transform.localScale != Vector3.one) transform.localScale = Vector3.one;

        if (controlLocked) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (!facingLock)
        {
            if (moveInput.x > 0.01f) currentFacing = 1;
            else if (moveInput.x < -0.01f) currentFacing = -1;
        }

        if (visual != null)
        {
            // Eger Unity'deki bazi objelerin X ekseni zaten yone ters bakiyorsa objeyi tam tersine dondurebilir
            // currentFacing ile localScale x duzgun ayarlanmistir
            float flipMultiplier = invertVisualFacing ? -1f : 1f;
            visual.localScale = new Vector3(Facing * flipMultiplier, 1f, 1f);
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();

    public void OnJump(InputValue v)
    {
        if (v.isPressed) jumpPressed = true;
    }
}
