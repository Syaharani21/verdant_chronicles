using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private Vector2 detectionSize;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private AudioClip attackSound1; // SFX untuk serangan pertama
    [SerializeField] private AudioClip attackSound2; // SFX untuk serangan kedua

    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;

    private Animator anim;
    private Transform player;
    private bool isPlayerDetected = false;
    private bool isAttacking = false;
    private float cooldownTimer = Mathf.Infinity;
    private Health bossHealth;
    private bool isFlipped = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossHealth = GetComponent<Health>(); // Menggunakan skrip Health
    }

    private void Update()
    {
        if (bossHealth.IsDead) return;

        cooldownTimer += Time.deltaTime;

        if (!isPlayerDetected)
        {
            isPlayerDetected = DetectPlayer();
        }
        else
        {
            LookAtPlayer();

            if (!isAttacking)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance > attackSize.x)
                {
                    ChasePlayer();
                }
                else
                {
                    StartCoroutine(PerformAttackSequence());
                }
            }
        }
    }

    private bool DetectPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapBox(detectionPoint.position, detectionSize, 0f, playerLayer);
        return playerInRange != null;
    }

    private void ChasePlayer()
    {
        anim.SetBool("isRunning", true);
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private IEnumerator PerformAttackSequence()
    {
        isAttacking = true;

        // Serangan pertama
        anim.SetTrigger("isAttacking1");
        yield return new WaitForSeconds(0.5f);

        if (attackSound1 != null) SoundManager.instance.PlaySound(attackSound1); // SFX untuk serangan pertama
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, attackLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<Health>()?.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.5f);

        // Serangan kedua
        anim.SetTrigger("isAttacking2");
        yield return new WaitForSeconds(0.5f);

        if (attackSound2 != null) SoundManager.instance.PlaySound(attackSound2); // SFX untuk serangan kedua
        hitPlayers = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, attackLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<Health>()?.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown - 1.5f);
        isAttacking = false;
    }

    private void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(detectionPoint.position, detectionSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
