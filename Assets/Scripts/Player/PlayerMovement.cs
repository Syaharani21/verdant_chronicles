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
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set player size to 0.5
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
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); 
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f); 


        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());
        
        if (IsGrounded())
            jumpCount = 0;

        //Only move and jump when not dashing
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
        // Start dash
        isDashing = true;
        dashTime = 0;
        dashCooldownTime = dashCooldown; 
        
        anim.SetTrigger("dash"); 

        // Set dash speed
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
}