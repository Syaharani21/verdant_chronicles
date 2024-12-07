using UnityEngine;

public class player : MonoBehaviour // Capitalized class name
{
   public DialogManager dialogManager; 
    public GameObject dialogIcon;       
    private bool hasTalked = false;     
    private const float MoveSpeed = 10f; 

    private Rigidbody2D rb; 

    private void Start()
    {
       
        if (dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        }
        rb = GetComponent<Rigidbody2D>(); 
    }
    private void Update() {
         Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        rb.MovePosition(rb.position + input.normalized * (MoveSpeed * Time.fixedDeltaTime));
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !hasTalked)
        {
            if (dialogIcon != null)
            {
                dialogIcon.SetActive(false); 
            }
            if (dialogManager != null)
            {
                dialogManager.StartDialog(); 
            }
            hasTalked = true; 
        }
    }
}  