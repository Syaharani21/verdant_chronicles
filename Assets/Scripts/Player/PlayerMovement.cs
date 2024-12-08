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

    // Menambahkan tiga referensi ke DialogManager
    public DialogManager dialogManager1; // Referensi DialogManager pertama
    public DialogManager dialogManager2; // Referensi DialogManager kedua
    public DialogManager dialogManager3; // Referensi DialogManager ketiga

    public GameObject dialogIcon;       // Ikon untuk menunjukkan bahwa dialog tersedia
    private bool hasTalked = false;

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
    [SerializeField] private AudioClip stepSound; // Suara langkah
    [SerializeField] private float stepInterval = 0.5f; // Jeda langkah
    private float stepTimer = 0f; // Timer untuk langkah

    private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 2;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set player size to 0.5
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void Update()
    {
        // Cek jika salah satu dari dialog manager aktif
        if ((dialogManager1 != null && dialogManager1.IsDialogActive) ||
            (dialogManager2 != null && dialogManager2.IsDialogActive) ||
            (dialogManager3 != null && dialogManager3.IsDialogActive))
        {
            anim.SetBool("run", false); // Set animasi idle
            body.velocity = Vector2.zero; // Hentikan pergerakan
            return; // Jangan lanjutkan input atau pergerakan saat dialog aktif
        }

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

        // Hanya bergerak dan melompat saat tidak dashing dan tidak ada dialog
        if (!isDashing)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
                    SoundManager.instance.PlaySound(jumpSound);
            }

            if ((horizontalInput != 0) && Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0)
            {
                StartDash();
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    SoundManager.instance.PlaySound(dashSound);
            }
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

            // Mainkan suara setiap kali pemain melompat
            if (jumpSound != null)
            {
                SoundManager.instance.PlaySound(jumpSound);
            }
        }
    }

    private void StartDash()
    {
        // Mulai dash
        isDashing = true;
        dashTime = 0;
        dashCooldownTime = dashCooldown;
        anim.SetTrigger("dash");

        // Set kecepatan dash
        body.velocity = new Vector2(dashSpeed * Mathf.Sign(horizontalInput), body.velocity.y);
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
                SoundManager.instance.PlaySound(stepSound);
                stepTimer = 0; // Reset timer
            }
        }
        else
        {
            stepTimer = 0; // Reset timer jika tidak bergerak atau di udara
        }
    }

    private void Start()
    {
        // Pastikan ikon dialog aktif saat awal permainan
        if (dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika collider adalah NPC dan dialog belum pernah terjadi
        if (other.CompareTag("NPC") && !hasTalked)
        {
            if (dialogIcon != null)
            {
                dialogIcon.SetActive(false); // Sembunyikan ikon dialog
            }

            // Mulai dialog menggunakan salah satu DialogManager
            if (dialogManager1 != null && !dialogManager1.IsDialogActive)
            {
                dialogManager1.StartDialog(); // Memulai dialog manager pertama
            }
            else if (dialogManager2 != null && !dialogManager2.IsDialogActive)
            {
                dialogManager2.StartDialog(); // Memulai dialog manager kedua
            }
            else if (dialogManager3 != null && !dialogManager3.IsDialogActive)
            {
                dialogManager3.StartDialog(); // Memulai dialog manager ketiga
            }

            hasTalked = true; // Tandai bahwa dialog sudah terjadi
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cek jika collider adalah NPC
        if (other.CompareTag("NPC"))
        {
            // Pastikan dialogIcon tetap tidak muncul setelah berbicara
            if (dialogIcon != null && hasTalked)
            {
                dialogIcon.SetActive(false);
            }
        }
    }
}
