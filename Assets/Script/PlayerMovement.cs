using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    [Header("Dash")]
    private float horizontalInput;
    private bool isDashing;
    private float dashTime;
    private float dashCooldownTime;
    public bool IsDashing { get { return isDashing; } }

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private float stepInterval = 0.5f;
    private float stepTimer = 0f;

    private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 2;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set ukuran karakter
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void Update()
    {
        HandleInput();
        ProcessDash();
        ManageCooldowns();
        HandleStepSound();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());

        if (IsGrounded())
            jumpCount = 0;

        // Gerakan dan aksi hanya jika tidak sedang dash
        if (!isDashing)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
                Jump();

            if (horizontalInput != 0 && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0)
                StartDash();
        }
    }

    private void Move()
    {
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
    }

    private void Jump()
    {
        if (jumpCount < maxJumpCount)
        {
            anim.SetTrigger("jump");
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            jumpCount++;

            if (jumpSound != null)
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = 0;
        dashCooldownTime = dashCooldown;
        anim.SetTrigger("dash");

        body.velocity = new Vector2(dashSpeed * Mathf.Sign(horizontalInput), body.velocity.y);

        if (dashSound != null)
            AudioSource.PlayClipAtPoint(dashSound, transform.position);
    }

    private void ProcessDash()
    {
        if (isDashing)
        {
            dashTime += Time.deltaTime;

            if (dashTime >= dashDuration)
            {
                isDashing = false;
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            }
        }
    }

    private void ManageCooldowns()
    {
        if (dashCooldownTime > 0)
            dashCooldownTime -= Time.deltaTime;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool CanAttack()
    {
        return !OnWall() && !isDashing;
    }

    private void HandleStepSound()
    {
        if (IsGrounded() && Mathf.Abs(horizontalInput) > 0.01f && !isDashing)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                if (stepSound != null)
                    AudioSource.PlayClipAtPoint(stepSound, transform.position);
                stepTimer = 0;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }
}
