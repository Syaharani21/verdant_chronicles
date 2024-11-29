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
    [SerializeField] private AudioClip rangedSound;  
    [SerializeField] private AudioClip meleeattackSound;  
    [SerializeField] private AudioClip weaponSwitchSound; 

    private Animator anim;                              
    private PlayerMovement playerMovement;              
    private float cooldownTimer = Mathf.Infinity;       
    private bool isMeleeAttack = false;                 
    private bool isAttacking = false; // Mencegah serangan berulang selama animasi

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Switch antara melee dan ranged attack dengan SFX
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isMeleeAttack = false;
            PlayWeaponSwitchSound();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isMeleeAttack = true;
            PlayWeaponSwitchSound();
        }

        // Jika serangan diaktifkan, dan cooldown selesai
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.CanAttack() && !isAttacking)
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
        isAttacking = true; // Memulai siklus serangan
        SoundManager.instance.PlaySound(rangedSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        yield return new WaitForSeconds(0.1f);

        fireballs[FindFireball()].transform.position = firePoint.position;
        fireballs[FindFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));

        yield return new WaitForSeconds(attackCooldown); // Tunggu hingga cooldown selesai
        isAttacking = false; // Akhiri siklus serangan
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true; // Memulai siklus serangan
        SoundManager.instance.PlaySound(meleeattackSound);
        anim.SetTrigger("MeleeAttack");
        cooldownTimer = 0;

        // Tunggu hingga animasi mencapai titik hit
        yield return new WaitForSeconds(0.2f); 

        // Hit detection
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Enemy terkena serangan: " + enemy.name);
            enemy.GetComponent<Health>()?.TakeDamage(meleeDamage);
        }

        // Tunggu hingga animasi selesai sepenuhnya
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false; // Akhiri siklus serangan
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy) return i;
        }
        return 0;
    }

    private void PlayWeaponSwitchSound()
    {
        if (SoundManager.instance != null && weaponSwitchSound != null)
        {
            SoundManager.instance.PlaySound(weaponSwitchSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, meleeAttackRange);
        }
    }
}
