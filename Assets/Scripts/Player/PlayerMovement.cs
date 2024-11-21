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

    private float horizontalInput;
    private bool isDashing;
    private float dashTime;
    private float dashCooldownTime;
    public bool IsDashing { get { return isDashing; } }

    private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 2;

    private void Awake()
    {
        // Inisialisasi komponen
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set ukuran pemain menjadi 0.5 dari ukuran asli
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void Update()
    {
        HandleInput();
        ProcessDash();
        ManageCooldowns();
    }

    private void HandleInput()
    {
        // Tangkap input horizontal
        horizontalInput = Input.GetAxis("Horizontal");

        // Flip arah pemain dengan ukuran 0.5f
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); 
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f); 

        // Update animasi berjalan dan posisi grounded
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());
        
        if (IsGrounded())
            jumpCount = 0;

        // Hanya bergerak dan melompat jika tidak sedang melakukan dash
        if (!isDashing)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();

            
            if ((horizontalInput != 0) && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0)
                StartDash();
        }
    }

    private void Move()
    {
        // Menggerakkan pemain ke arah horizontal jika tidak sedang dash
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
    }

    private void Jump()
    {
        
        if (jumpCount < maxJumpCount)
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
            jumpCount++;
        }
    }

    private void StartDash()
    {
        // Memulai dash
        isDashing = true;
        dashTime = 0;
        dashCooldownTime = dashCooldown; 
        
        anim.SetTrigger("dash"); 

        // Mengatur kecepatan dash
        body.velocity = new Vector2(dashSpeed * Mathf.Sign(horizontalInput), body.velocity.y);
    }

    private void ProcessDash()
    {
        if (isDashing)
        {
            dashTime += Time.deltaTime;

            // Hentikan dash setelah durasi dash berakhir
            if (dashTime >= dashDuration)
            {
                isDashing = false;
                
                
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            }
        }
    }

    private void ManageCooldowns()
    {
        // Mengurangi waktu cooldown dash
        if (dashCooldownTime > 0)
            dashCooldownTime -= Time.deltaTime;
    }

    private bool IsGrounded()
    {
        // Mengecek apakah pemain berada di tanah dengan BoxCast
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        // Mengecek apakah pemain sedang menempel di dinding
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool CanAttack()
    {
        return !OnWall() && !isDashing;
    }
}
