using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private float meleeAttackRange;
    [SerializeField] private int meleeDamage;
    [SerializeField] private LayerMask enemyLayer;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private bool isMeleeAttack = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Switch between melee and ranged attacks
        if (Input.GetKeyDown(KeyCode.Alpha1)) isMeleeAttack = false;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) isMeleeAttack = true;

        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.CanAttack())
        {
            if (isMeleeAttack)
                StartCoroutine(MeleeAttack());
            else
                StartCoroutine(RangedAttack());
        }

        cooldownTimer += Time.deltaTime;
    }

    private IEnumerator RangedAttack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        yield return new WaitForSeconds(0.1f);

        fireballs[FindFireball()].transform.position = firePoint.position;
        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private IEnumerator MeleeAttack()
    {
        anim.SetTrigger("MeleeAttack");
        cooldownTimer = 0;

        yield return new WaitForSeconds(0.1f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to the enemy
            //enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
        }
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy) return i;
        }
        return 0;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize melee attack range
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, meleeAttackRange);
        }
    }
}
