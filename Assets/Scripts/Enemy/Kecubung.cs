using UnityEngine;

public class Kecubung : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private int damage = 1; 
    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance = 1f; 
    [SerializeField] private BoxCollider2D boxCollider; 
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; 
    private float cooldownTimer = Mathf.Infinity; 

    private Health playerHealth; 
    private void Update()
    {
        
        cooldownTimer += Time.deltaTime;

       
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                DamagePlayer(); 
            }
        }
    }

    private bool PlayerInSight()
    {
        
        Vector3 boxCastDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + boxCastDirection * range * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0,
            boxCastDirection,
            0,
            playerLayer
        );

       
        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void DamagePlayer()
    {
       
        if (PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Player damaged for " + damage + " points!");
        }
    }

    private void OnDrawGizmos()
    {
        // RedBox untuk memvisualisasikan BoxCast
        Gizmos.color = Color.red;
        Vector3 boxCastDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + boxCastDirection * range * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z)
        );
    }
}