using System.Collections;
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
    [SerializeField] private int fireballDamage;

    // Menambahkan referensi ke DialogManager
    [SerializeField] private DialogManager[] dialogManagers;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private bool isMeleeAttack = false;
    private bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Pastikan dialogManagers sudah diatur melalui Inspector
        if (dialogManagers.Length == 0)
        {
            Debug.LogError("DialogManager belum diatur di PlayerAttack!");
        }
    }

    private void Update()
    {
        // Cek apakah salah satu dialog aktif, jika aktif maka hentikan serangan
        if (IsAnyDialogActive())
        {
            return; // Jangan lakukan apa pun selama dialog berlangsung
        }

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

        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.CanAttack() && !isAttacking)
        {
            if (isMeleeAttack)
                StartCoroutine(MeleeAttack());
            else
                StartCoroutine(RangedAttack());
        }

        cooldownTimer += Time.deltaTime;
    }

    private bool IsAnyDialogActive()
    {
        foreach (var dialogManager in dialogManagers)
        {
            if (dialogManager != null && dialogManager.IsDialogActive)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator RangedAttack()
    {
        isAttacking = true;
        SoundManager.instance.PlaySound(rangedSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        yield return new WaitForSeconds(0.1f);

        var fireball = fireballs[FindFireball()];
        fireball.transform.position = firePoint.position;
        fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        fireball.GetComponent<Projectile>().SetDamage(fireballDamage); // Set damage dari PlayerAttack

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private IEnumerator MeleeAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        SoundManager.instance.PlaySound(meleeattackSound);
        anim.SetTrigger("MeleeAttack");
        cooldownTimer = 0;

        yield return new WaitForSeconds(0.30f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, meleeAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(meleeDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
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
