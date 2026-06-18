// ============================================================
//  EnemyFSM.cs
//  Finite State Machine Adaptif untuk musuh Verdant Chronicles.
//
//  State:   Patrol → Chase → Attack → Retreat → Dead
//  Adaptasi: agresivitas, cooldown, kecepatan kejar, pola
//            serangan menyesuaikan perilaku player secara real-time.
//
//  Cara pakai:
//  1. Pasang komponen ini + Health di Enemy GameObject.
//  2. Isi semua referensi di Inspector.
//  3. Hapus EnemyPatrol dan Kecubung dari GameObject yang sama.
// ============================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyFSM : MonoBehaviour
{
    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Patrol
    // ════════════════════════════════════════════════════════
    [Header("Patrol")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float patrolSpeed = 2f;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Deteksi
    // ════════════════════════════════════════════════════════
    [Header("Deteksi Player")]
    [SerializeField] private float detectionRange   = 6f;  // Jarak deteksi normal
    [SerializeField] private float losePlayerRange  = 9f;  // Jarak kehilangan player
    [SerializeField] private LayerMask playerLayer;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Chase
    // ════════════════════════════════════════════════════════
    [Header("Chase")]
    [SerializeField] private float chaseSpeed       = 4f;
    [SerializeField] private float chaseSpeedMax    = 7f;  // Batas atas kecepatan adaptif

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Attack
    // ════════════════════════════════════════════════════════
    [Header("Attack")]
    [SerializeField] private Transform  attackPoint;
    [SerializeField] private float      attackRange       = 1.5f;
    [SerializeField] private int        attackDamage      = 10;
    [SerializeField] private float      attackCooldown    = 1.5f;
    [SerializeField] private float      attackCooldownMin = 0.5f; // Batas bawah cooldown adaptif
    [SerializeField] private AudioClip  attackSound;

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — Retreat
    // ════════════════════════════════════════════════════════
    [Header("Retreat")]
    [Tooltip("Persentase HP untuk masuk Retreat (0–1)")]
    [SerializeField] private float retreatHpThreshold = 0.25f;
    [SerializeField] private float retreatSpeed       = 5f;
    [SerializeField] private float retreatDuration    = 2f;   // Berapa lama mundur

    // ════════════════════════════════════════════════════════
    //  INSPECTOR — SFX
    // ════════════════════════════════════════════════════════
    [Header("SFX")]
    [SerializeField] private AudioClip alertSound;  // Suara saat mendeteksi player
    [SerializeField] private AudioClip hurtSound;   // Suara kena hit (opsional override)

    // ════════════════════════════════════════════════════════
    //  RUNTIME — privat
    // ════════════════════════════════════════════════════════
    private EnemyState    currentState = EnemyState.Patrol;
    private EnemyAdaptiveData adaptData = new EnemyAdaptiveData();

    private Health        health;
    private Rigidbody2D   rb;
    private Animator      anim;
    private SpriteRenderer sr;

    private Transform     player;
    private Health        playerHealth;

    private bool   movingLeft         = true;
    private float  attackCooldownTimer;
    private float  retreatTimer;
    private bool   isAttacking        = false;
    private float  currentChaseSpeed;
    private float  currentAttackCooldown;

    // Ekspos state saat ini (untuk debug / UI)
    public EnemyState CurrentState => currentState;
    public EnemyAdaptiveData AdaptData => adaptData;

    // ════════════════════════════════════════════════════════
    //  UNITY LIFECYCLE
    // ════════════════════════════════════════════════════════
    private void Awake()
    {
        health = GetComponent<Health>();
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponent<Animator>();
        sr     = GetComponent<SpriteRenderer>();

        currentChaseSpeed      = chaseSpeed;
        currentAttackCooldown  = attackCooldown;
        attackCooldownTimer    = attackCooldown; // siap serang langsung
    }

    private void Start()
    {
        // Cari player di scene
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

        // ── Perbarui adaptasi tiap frame ──────────────────
        AdaptBehavior();

        // ── Jalankan state saat ini ───────────────────────
        switch (currentState)
        {
            case EnemyState.Patrol:  UpdatePatrol();  break;
            case EnemyState.Chase:   UpdateChase();   break;
            case EnemyState.Attack:  UpdateAttack();  break;
            case EnemyState.Retreat: UpdateRetreat(); break;
            case EnemyState.Dead:                     break;
        }
    }

    // ════════════════════════════════════════════════════════
    //  STATE: PATROL
    // ════════════════════════════════════════════════════════
    private void UpdatePatrol()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isChasing", false);

        // Gerak bolak-balik
        if (movingLeft)
        {
            if (transform.position.x > leftEdge.position.x)
                MoveInDirection(-1, patrolSpeed);
            else
                movingLeft = false;
        }
        else
        {
            if (transform.position.x < rightEdge.position.x)
                MoveInDirection(1, patrolSpeed);
            else
                movingLeft = true;
        }

        // Transisi → Chase jika player terdeteksi
        if (PlayerInRange(detectionRange))
            TransitionTo(EnemyState.Chase);
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

        anim.SetBool("isRunning", true);
        anim.SetBool("isChasing", true);

        // Hadap ke player
        FaceTarget(player.position);

        float dist = DistToPlayer();

        // Transisi → Attack jika dalam jangkauan
        if (dist <= attackRange)
        {
            TransitionTo(EnemyState.Attack);
            return;
        }

        // Transisi → Patrol jika player terlalu jauh
        if (dist > losePlayerRange)
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        // Cek Retreat (HP rendah + aggression cukup tinggi)
        if (ShouldRetreat())
        {
            TransitionTo(EnemyState.Retreat);
            return;
        }

        // Gerak mendekati player
        MoveTowards(player.position, currentChaseSpeed);
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
        anim.SetBool("isChasing", false);
        FaceTarget(player.position);

        float dist = DistToPlayer();

        // Player kabur → Chase
        if (dist > attackRange * 1.5f)
        {
            TransitionTo(EnemyState.Chase);
            return;
        }

        // Cek Retreat
        if (ShouldRetreat())
        {
            TransitionTo(EnemyState.Retreat);
            return;
        }

        // Serang jika cooldown selesai dan tidak sedang menyerang
        if (!isAttacking && attackCooldownTimer >= currentAttackCooldown)
        {
            StartCoroutine(PerformAttack());
        }
    }

    // ════════════════════════════════════════════════════════
    //  STATE: RETREAT
    // ════════════════════════════════════════════════════════
    private void UpdateRetreat()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isChasing", false);

        retreatTimer -= Time.deltaTime;

        // Lari menjauhi player
        if (player != null)
        {
            float dir = transform.position.x < player.position.x ? -1f : 1f;
            MoveInDirection((int)dir, retreatSpeed);
        }

        // Selesai retreat → Chase kembali jika player masih dekat
        if (retreatTimer <= 0f)
        {
            if (PlayerInRange(detectionRange))
                TransitionTo(EnemyState.Chase);
            else
                TransitionTo(EnemyState.Patrol);
        }
    }

    // ════════════════════════════════════════════════════════
    //  SERANGAN (Coroutine)
    // ════════════════════════════════════════════════════════
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        attackCooldownTimer = 0f;

        // ── Pola serangan adaptif berdasarkan aggressionLevel ──
        switch (adaptData.aggressionLevel)
        {
            case 0:
            case 1:
                // Serangan tunggal biasa
                yield return StartCoroutine(SingleAttack());
                break;

            case 2:
                // Serangan ganda dengan jeda pendek
                yield return StartCoroutine(SingleAttack());
                yield return new WaitForSeconds(0.3f);
                if (DistToPlayer() <= attackRange * 1.2f)
                    yield return StartCoroutine(SingleAttack());
                break;

            case 3:
                // Combo tiga serangan, lalu mundur sebentar
                yield return StartCoroutine(SingleAttack());
                yield return new WaitForSeconds(0.25f);
                if (DistToPlayer() <= attackRange * 1.2f)
                    yield return StartCoroutine(SingleAttack());
                yield return new WaitForSeconds(0.25f);
                if (DistToPlayer() <= attackRange * 1.4f)
                    yield return StartCoroutine(SingleAttack());
                // Micro-retreat setelah combo
                yield return StartCoroutine(MicroRetreat(0.4f));
                break;
        }

        isAttacking = false;
    }

    private IEnumerator SingleAttack()
    {
        anim.SetTrigger("attack");
        SoundManager.instance.PlaySound(attackSound);

        // Delay sebelum hitbox aktif (sesuaikan dengan animasi)
        yield return new WaitForSeconds(0.3f);

        // Deteksi hit
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint != null ? attackPoint.position : transform.position,
            attackRange,
            playerLayer
        );
        foreach (var hit in hits)
            hit.GetComponent<Health>()?.TakeDamage(attackDamage);
    }

    private IEnumerator MicroRetreat(float duration)
    {
        float t = 0f;
        float dir = transform.position.x < (player != null ? player.position.x : transform.position.x) ? -1f : 1f;
        while (t < duration)
        {
            MoveInDirection((int)dir, retreatSpeed * 0.6f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    // ════════════════════════════════════════════════════════
    //  ADAPTASI PERILAKU
    //  Dipanggil tiap frame; menyesuaikan parameter runtime
    //  berdasarkan EnemyAdaptiveData.
    // ════════════════════════════════════════════════════════
    private void AdaptBehavior()
    {
        int lvl = adaptData.aggressionLevel;

        // ── Kecepatan kejar naik dengan agresivitas ──────
        currentChaseSpeed = Mathf.Lerp(
            chaseSpeed,
            chaseSpeedMax,
            lvl / 3f
        );

        // ── Cooldown serangan turun dengan agresivitas ───
        currentAttackCooldown = Mathf.Lerp(
            attackCooldown,
            attackCooldownMin,
            lvl / 3f
        );

        // ── Jangkauan deteksi sedikit melebar jika agresif ──
        // (tidak perlu variabel karena langsung dipakai di PlayerInRange)
    }

    // ════════════════════════════════════════════════════════
    //  TRANSISI STATE
    // ════════════════════════════════════════════════════════
    private void TransitionTo(EnemyState next)
    {
        if (currentState == next) return;

        // OnExit state lama
        switch (currentState)
        {
            case EnemyState.Retreat:
                retreatTimer = 0f;
                break;
        }

        currentState = next;

        // OnEnter state baru
        switch (next)
        {
            case EnemyState.Chase:
                SoundManager.instance.PlaySound(alertSound);
                break;

            case EnemyState.Retreat:
                retreatTimer = retreatDuration;
                anim.SetTrigger("hurt"); // Pakai animasi hurt untuk efek "kaget"
                break;

            case EnemyState.Dead:
                anim.SetBool("isRunning", false);
                anim.SetBool("isChasing", false);
                rb.velocity = Vector2.zero;
                enabled = false;
                break;
        }

        Debug.Log($"[FSM] {gameObject.name}: {currentState}");
    }

    // ════════════════════════════════════════════════════════
    //  EVENT — dipanggil dari Health / luar
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Panggil ini dari Health.TakeDamage jika ingin musuh adaptif.
    /// Atau hubungkan via UnityEvent di komponen Health.
    /// </summary>
    public void OnHitByPlayer(float damage)
    {
        adaptData.RegisterPlayerHit(damage);

        // Langsung Chase jika sedang Patrol dan kena serangan
        if (currentState == EnemyState.Patrol)
            TransitionTo(EnemyState.Chase);
    }

    /// <summary>
    /// Panggil ini tiap kali player melakukan dash (dari PlayerMovement).
    /// Digunakan untuk mendeteksi pola dodge player.
    /// </summary>
    public void OnPlayerDash()
    {
        adaptData.RegisterPlayerDash();
    }

    // ════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════
    private bool PlayerInRange(float range)
    {
        if (player == null) return false;
        // Sedikit perlebar jangkauan jika agresif
        float bonus = adaptData.aggressionLevel * 0.5f;
        return DistToPlayer() <= range + bonus;
    }

    private float DistToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }

    private bool ShouldRetreat()
    {
        if (adaptData.aggressionLevel < 2) return false; // Hanya level 2+ yang bisa retreat
        float hpRatio = health.currentHealth / health.StartingHealth;
        return hpRatio <= retreatHpThreshold;
    }

    private void MoveInDirection(int dir, float speed)
    {
        // Flip sprite
        sr.flipX = dir < 0;

        Vector3 pos = transform.position;
        pos.x += dir * speed * Time.deltaTime;

        // Batasi dalam area patrol (hanya saat patrol)
        if (currentState == EnemyState.Patrol)
            pos.x = Mathf.Clamp(pos.x, leftEdge.position.x, rightEdge.position.x);

        transform.position = pos;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        int dir = target.x > transform.position.x ? 1 : -1;
        MoveInDirection(dir, speed);
    }

    private void FaceTarget(Vector3 target)
    {
        sr.flipX = target.x < transform.position.x;
    }

    // ════════════════════════════════════════════════════════
    //  GIZMOS
    // ════════════════════════════════════════════════════════
    private void OnDrawGizmosSelected()
    {
        // Jangkauan deteksi
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Jangkauan kehilangan player
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);

        // Jangkauan serangan
        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        else
            Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
