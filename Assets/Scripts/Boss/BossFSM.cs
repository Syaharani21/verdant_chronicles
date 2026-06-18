// ============================================================
//  BossFSM.cs
//  FSM Adaptif Boss — Verdant Chronicles
//
//  3 Fase berdasarkan HP:
//    Fase 1 (HP > 66%) : Pola normal
//    Fase 2 (HP 33–66%): Lebih agresif, tambah serangan proyektil
//    Fase 3 (HP < 33%) : Combo ekstrem, kecepatan maks, enrage
//
//  State: Idle → Chase → Attack → Retreat → Enrage → Dead
// ============================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Animator))]
public class BossFSM : MonoBehaviour
{
    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Deteksi
    // ════════════════════════════════════════════════════════
    [Header("Deteksi")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private Vector2   detectionSize = new Vector2(16f, 4f);
    [SerializeField] private LayerMask playerLayer;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Gerakan
    // ════════════════════════════════════════════════════════
    [Header("Gerakan")]
    [SerializeField] private float speedPhase1 = 2.5f;
    [SerializeField] private float speedPhase2 = 4f;
    [SerializeField] private float speedPhase3 = 6f;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Serangan
    // ════════════════════════════════════════════════════════
    [Header("Serangan")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2   attackSize    = new Vector2(2f, 2f);
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private int       damagePhase1  = 15;
    [SerializeField] private int       damagePhase2  = 20;
    [SerializeField] private int       damagePhase3  = 30;
    [SerializeField] private float     cooldownPhase1 = 2.2f;
    [SerializeField] private float     cooldownPhase2 = 1.5f;
    [SerializeField] private float     cooldownPhase3 = 0.8f;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Proyektil (Fase 2 & 3)
    // ════════════════════════════════════════════════════════
    [Header("Proyektil (Fase 2+)")]
    [SerializeField] private GameObject[] projectilePrefabs; // Pool proyektil boss
    [SerializeField] private Transform    projectileSpawnPoint;
    [SerializeField] private float        projectileSpeed = 8f;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Retreat
    // ════════════════════════════════════════════════════════
    [Header("Retreat")]
    [SerializeField] private float retreatHpThreshold = 0.15f; // 15% HP
    [SerializeField] private float retreatDuration    = 1.5f;
    [SerializeField] private float retreatSpeed       = 6f;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — SFX
    // ════════════════════════════════════════════════════════
    [Header("SFX")]
    [SerializeField] private AudioClip attackSound1;
    [SerializeField] private AudioClip attackSound2;
    [SerializeField] private AudioClip attackSound3;   // Serangan Fase 3
    [SerializeField] private AudioClip enrageSound;
    [SerializeField] private AudioClip phaseTransitionSound;

    // ════════════════════════════════════════════════════════
    //  RUNTIME
    // ════════════════════════════════════════════════════════
    private EnemyState   currentState = EnemyState.Patrol; // Gunakan Patrol sebagai "Idle"
    private int          currentPhase = 1;
    private bool         phase2Triggered = false;
    private bool         phase3Triggered = false;

    private Health       health;
    private Animator     anim;
    private SpriteRenderer sr;

    private Transform    player;
    private Health       playerHealth;

    private bool         isPlayerDetected = false;
    private bool         isAttacking      = false;
    private bool         isFlipped        = false;
    private float        attackCooldownTimer;
    private float        retreatTimer;

    private EnemyAdaptiveData adaptData = new EnemyAdaptiveData();

    public EnemyState CurrentState => currentState;
    public int        CurrentPhase => currentPhase;

    // ════════════════════════════════════════════════════════
    //  UNITY LIFECYCLE
    // ════════════════════════════════════════════════════════
    private void Awake()
    {
        health = GetComponent<Health>();
        anim   = GetComponent<Animator>();
        sr     = GetComponent<SpriteRenderer>();
        attackCooldownTimer = cooldownPhase1;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (health.IsDead)
        {
            TransitionTo(EnemyState.Dead);
            return;
        }

        attackCooldownTimer += Time.deltaTime;

        UpdatePhase();

        switch (currentState)
        {
            case EnemyState.Patrol:  UpdateIdle();    break; // Idle/menunggu
            case EnemyState.Chase:   UpdateChase();   break;
            case EnemyState.Attack:  UpdateAttack();  break;
            case EnemyState.Retreat: UpdateRetreat(); break;
            case EnemyState.Dead:                     break;
        }
    }

    // ════════════════════════════════════════════════════════
    //  UPDATE FASE
    // ════════════════════════════════════════════════════════
    private void UpdatePhase()
    {
        if (health.StartingHealth <= 0) return;
        float ratio = health.currentHealth / health.StartingHealth;

        if (ratio <= 0.33f && !phase3Triggered)
        {
            phase3Triggered = true;
            currentPhase = 3;
            OnPhaseTransition(3);
        }
        else if (ratio <= 0.66f && !phase2Triggered)
        {
            phase2Triggered = true;
            currentPhase = 2;
            OnPhaseTransition(2);
        }
    }

    private void OnPhaseTransition(int phase)
    {
        SoundManager.instance.PlaySound(phaseTransitionSound);
        anim.SetTrigger("phaseTransition");
        Debug.Log($"[BossFSM] Memasuki Fase {phase}!");

        if (phase == 3)
        {
            // Enrage: boss berubah warna sedikit
            if (sr != null) sr.color = new Color(1f, 0.6f, 0.6f);
            SoundManager.instance.PlaySound(enrageSound);
        }
    }

    // ════════════════════════════════════════════════════════
    //  STATE: IDLE (Patrol)
    // ════════════════════════════════════════════════════════
    private void UpdateIdle()
    {
        anim.SetBool("isRunning", false);

        if (!isPlayerDetected)
        {
            Collider2D hit = Physics2D.OverlapBox(
                detectionPoint != null ? detectionPoint.position : transform.position,
                detectionSize, 0f, playerLayer);
            if (hit != null)
            {
                isPlayerDetected = true;
                TransitionTo(EnemyState.Chase);
            }
        }
    }

    // ════════════════════════════════════════════════════════
    //  STATE: CHASE
    // ════════════════════════════════════════════════════════
    private void UpdateChase()
    {
        if (player == null || (playerHealth != null && playerHealth.IsDead))
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        float speed = PhaseSpeed();
        anim.SetBool("isRunning", true);
        LookAtPlayer();

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackSize.x)
        {
            TransitionTo(EnemyState.Attack);
            return;
        }

        if (ShouldRetreat())
        {
            TransitionTo(EnemyState.Retreat);
            return;
        }

        Vector3 target = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // ════════════════════════════════════════════════════════
    //  STATE: ATTACK
    // ════════════════════════════════════════════════════════
    private void UpdateAttack()
    {
        if (player == null || (playerHealth != null && playerHealth.IsDead))
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        anim.SetBool("isRunning", false);
        LookAtPlayer();

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > attackSize.x * 1.6f)
        {
            TransitionTo(EnemyState.Chase);
            return;
        }

        if (ShouldRetreat())
        {
            TransitionTo(EnemyState.Retreat);
            return;
        }

        if (!isAttacking && attackCooldownTimer >= PhaseCooldown())
            StartCoroutine(PerformPhaseAttack());
    }

    // ════════════════════════════════════════════════════════
    //  STATE: RETREAT
    // ════════════════════════════════════════════════════════
    private void UpdateRetreat()
    {
        anim.SetBool("isRunning", true);
        retreatTimer -= Time.deltaTime;

        if (player != null)
        {
            float dir = transform.position.x < player.position.x ? -1f : 1f;
            transform.position += new Vector3(dir * retreatSpeed * Time.deltaTime, 0f, 0f);
            if (sr != null) sr.flipX = dir < 0;
        }

        if (retreatTimer <= 0f)
            TransitionTo(EnemyState.Chase);
    }

    // ════════════════════════════════════════════════════════
    //  POLA SERANGAN PER FASE
    // ════════════════════════════════════════════════════════
    private IEnumerator PerformPhaseAttack()
    {
        isAttacking = true;
        attackCooldownTimer = 0f;

        switch (currentPhase)
        {
            case 1:
                yield return StartCoroutine(MeleeSwing(attackSound1, damagePhase1));
                break;

            case 2:
                // Melee + proyektil
                yield return StartCoroutine(MeleeSwing(attackSound2, damagePhase2));
                yield return new WaitForSeconds(0.4f);
                yield return StartCoroutine(FireProjectile());
                break;

            case 3:
                // Combo 2 melee + 2 proyektil (enrage)
                yield return StartCoroutine(MeleeSwing(attackSound3, damagePhase3));
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(MeleeSwing(attackSound3, damagePhase3));
                yield return new WaitForSeconds(0.3f);
                yield return StartCoroutine(FireProjectile());
                yield return new WaitForSeconds(0.15f);
                yield return StartCoroutine(FireProjectile());
                break;
        }

        isAttacking = false;
    }

    private IEnumerator MeleeSwing(AudioClip sound, int damage)
    {
        anim.SetTrigger("isAttacking1");
        SoundManager.instance.PlaySound(sound);
        yield return new WaitForSeconds(0.45f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackPoint != null ? attackPoint.position : transform.position,
            attackSize, 0f, attackLayer);
        foreach (var h in hits)
            h.GetComponent<Health>()?.TakeDamage(damage);
    }

    private IEnumerator FireProjectile()
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0) yield break;

        anim.SetTrigger("isAttacking2");

        // Cari proyektil tidak aktif
        GameObject proj = null;
        foreach (var p in projectilePrefabs)
        {
            if (p != null && !p.activeInHierarchy) { proj = p; break; }
        }
        if (proj == null) yield break;

        Vector3 spawnPos = projectileSpawnPoint != null
            ? projectileSpawnPoint.position
            : transform.position;

        proj.transform.position = spawnPos;

        var projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            float dir = player != null
                ? Mathf.Sign(player.position.x - transform.position.x)
                : (isFlipped ? 1f : -1f);
            projectile.SetDirection(dir);
            projectile.SetDamage(damagePhase2);
        }

        yield return new WaitForSeconds(0.3f);
    }

    // ════════════════════════════════════════════════════════
    //  TRANSISI
    // ════════════════════════════════════════════════════════
    private void TransitionTo(EnemyState next)
    {
        if (currentState == next) return;
        currentState = next;

        switch (next)
        {
            case EnemyState.Retreat:
                retreatTimer = retreatDuration;
                break;

            case EnemyState.Dead:
                anim.SetBool("isRunning", false);
                enabled = false;
                break;
        }

        Debug.Log($"[BossFSM] State → {next} (Fase {currentPhase})");
    }

    // ════════════════════════════════════════════════════════
    //  EVENT
    // ════════════════════════════════════════════════════════
    public void OnHitByPlayer(float damage)
    {
        adaptData.RegisterPlayerHit(damage);
        if (currentState == EnemyState.Patrol)
            TransitionTo(EnemyState.Chase);
    }

    // ════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════
    private bool ShouldRetreat()
    {
        float ratio = health.currentHealth / health.StartingHealth;
        return ratio <= retreatHpThreshold;
    }

    private float PhaseSpeed()
    {
        return currentPhase == 3 ? speedPhase3
             : currentPhase == 2 ? speedPhase2
             : speedPhase1;
    }

    private float PhaseCooldown()
    {
        return currentPhase == 3 ? cooldownPhase3
             : currentPhase == 2 ? cooldownPhase2
             : cooldownPhase1;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;
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
        if (detectionPoint != null)
            Gizmos.DrawWireCube(detectionPoint.position, detectionSize);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}
