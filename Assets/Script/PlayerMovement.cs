using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    private Animator anim;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float horizontalInput;

    // Fixed scale values for the player
    private Vector3 originalScale;

    // Start is called before the first frame update
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Store the original scale
        originalScale = new Vector3(6.809174f, 5.37407f, 1);
    }

    // Update is called once per frame
    private void Update()
    {
        // Use the class-level horizontalInput variable
        horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if (horizontalInput > 0.01f)
        {
            // Face right and maintain the original scale
            transform.localScale = originalScale;
        }
        else if (horizontalInput < -0.01f)
        {
            // Face left (invert x-axis while keeping original y and z scales)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        // Jump when Space is pressed and player is grounded
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            Jump();
        }

        // Update animation parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
    }

    // Jump method
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("jump");
    }

    // Check if the player is grounded using BoxCast
    private bool isGrounded()
    {
        // Perform a BoxCast to check if the player is standing on the ground layer, including the Tilemap
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    // Check if the player can attack
    public bool canAttack()
    {
        // Now horizontalInput refers to the correct class-level variable
        return horizontalInput == 0 && isGrounded();
    }
}
