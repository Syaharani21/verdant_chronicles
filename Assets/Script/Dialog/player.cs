using UnityEngine;

public class player : MonoBehaviour // Capitalized class name
{
    [SerializeField] private dialogUi dialogueUi; // Capitalized class name
    private const float MoveSpeed = 10f;

    public dialogUi DialogueUi => dialogueUi; 

    public InteractionObject Interaction { get; set; }
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() // Use FixedUpdate for physics-related updates
    {
        if (dialogueUi.IsOpen)
        {
            return;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        rb.MovePosition(rb.position + input.normalized * (MoveSpeed * Time.fixedDeltaTime));

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interaction?.Interact(this); // Corrected method call
        }
    }
}