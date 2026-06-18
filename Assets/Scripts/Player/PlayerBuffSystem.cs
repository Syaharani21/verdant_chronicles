using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menyimpan data satu buff aktif.
/// </summary>
public class ActiveBuff
{
    public FlowerData data;
    public float remainingTime; // -1 = permanen (selama bunga ada di pot)
    public bool isPermanent => remainingTime < 0f;
}

/// <summary>
/// Komponen yang dipasang di Player.
/// Menerima buff dari sistem greenhouse, menerapkannya ke stats player,
/// dan menghapusnya saat bunga dicabut atau durasi habis.
/// </summary>
public class PlayerBuffSystem : MonoBehaviour
{
    public static PlayerBuffSystem instance { get; private set; }

    [Header("Referensi Komponen Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Health playerHealth;

    [Header("UI Buff (Opsional)")]
    [SerializeField] private Transform buffIconContainer; // Parent untuk ikon buff di HUD
    [SerializeField] private GameObject buffIconPrefab;   // Prefab ikon buff (Image + Text)

    // Stats base yang disimpan agar bisa dikembalikan
    private float baseSpeed;
    private float baseJumpPower;
    private float baseDashCooldown;
    private int baseMeleeDamage;
    private int baseFireballDamage;

    // Daftar buff aktif
    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    // Ikon buff di UI
    private Dictionary<ActiveBuff, GameObject> buffIcons = new Dictionary<ActiveBuff, GameObject>();

    // Coroutine regen HP
    private Coroutine healthRegenCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Ambil komponen otomatis jika belum diset di Inspector
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (playerAttack == null)   playerAttack   = GetComponent<PlayerAttack>();
        if (playerHealth == null)   playerHealth   = GetComponent<Health>();
    }

    private void Start()
    {
        // Simpan nilai base dari PlayerMovement via reflection-safe public accessor
        // Menggunakan serialized field default values — pastikan sama dengan Inspector
        baseSpeed           = playerMovement.BaseSpeed;
        baseJumpPower       = playerMovement.BaseJumpPower;
        baseDashCooldown    = playerMovement.BaseDashCooldown;
        baseMeleeDamage     = playerAttack.BaseMeleeDamage;
        baseFireballDamage  = playerAttack.BaseFireballDamage;
    }

    private void Update()
    {
        // Countdown buff berdurasi
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];
            if (buff.isPermanent) continue;

            buff.remainingTime -= Time.deltaTime;
            UpdateBuffIcon(buff);

            if (buff.remainingTime <= 0f)
            {
                RemoveBuffInternal(buff);
            }
        }
    }

    // ─────────────────────────────────────────────
    // PUBLIC API — dipanggil oleh Pot.cs
    // ─────────────────────────────────────────────

    /// <summary>
    /// Tambahkan buff dari bunga yang baru ditanam.
    /// isPermanent = true berarti buff aktif selama bunga ada di pot.
    /// </summary>
    public void ApplyBuff(FlowerData flower, bool isPermanent = true)
    {
        var buff = new ActiveBuff
        {
            data = flower,
            remainingTime = isPermanent ? -1f : flower.buffDuration
        };

        activeBuffs.Add(buff);
        ApplyStatChange(flower.buffType, flower.buffValue);
        SpawnBuffIcon(buff);

        Debug.Log($"[Buff] Diterapkan: {flower.flowerName} ({flower.buffType} +{flower.buffValue})");
    }

    /// <summary>
    /// Hapus buff dari bunga yang dicabut (cocokkan berdasarkan FlowerData).
    /// </summary>
    public void RemoveBuff(FlowerData flower)
    {
        var buff = activeBuffs.Find(b => b.data == flower);
        if (buff != null)
        {
            RemoveBuffInternal(buff);
            Debug.Log($"[Buff] Dihapus: {flower.flowerName}");
        }
    }

    // ─────────────────────────────────────────────
    // INTERNAL
    // ─────────────────────────────────────────────

    private void RemoveBuffInternal(ActiveBuff buff)
    {
        activeBuffs.Remove(buff);
        RevertStatChange(buff.data.buffType, buff.data.buffValue);
        DestroyBuffIcon(buff);
    }

    private void ApplyStatChange(BuffType type, float value)
    {
        switch (type)
        {
            case BuffType.SpeedBoost:
                playerMovement.SetSpeed(playerMovement.CurrentSpeed + value);
                break;

            case BuffType.JumpBoost:
                playerMovement.SetJumpPower(playerMovement.CurrentJumpPower + value);
                break;

            case BuffType.DashCooldownReduction:
                float newCooldown = Mathf.Max(0.1f, playerMovement.CurrentDashCooldown - value);
                playerMovement.SetDashCooldown(newCooldown);
                break;

            case BuffType.AttackBoost:
                playerAttack.SetMeleeDamage(playerAttack.CurrentMeleeDamage + (int)value);
                playerAttack.SetFireballDamage(playerAttack.CurrentFireballDamage + (int)value);
                break;

            case BuffType.HealthRegen:
                if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
                healthRegenCoroutine = StartCoroutine(RegenHealthLoop(value));
                break;
        }
    }

    private void RevertStatChange(BuffType type, float value)
    {
        switch (type)
        {
            case BuffType.SpeedBoost:
                playerMovement.SetSpeed(Mathf.Max(baseSpeed, playerMovement.CurrentSpeed - value));
                break;

            case BuffType.JumpBoost:
                playerMovement.SetJumpPower(Mathf.Max(baseJumpPower, playerMovement.CurrentJumpPower - value));
                break;

            case BuffType.DashCooldownReduction:
                playerMovement.SetDashCooldown(Mathf.Min(baseDashCooldown, playerMovement.CurrentDashCooldown + value));
                break;

            case BuffType.AttackBoost:
                playerAttack.SetMeleeDamage(Mathf.Max(baseMeleeDamage, playerAttack.CurrentMeleeDamage - (int)value));
                playerAttack.SetFireballDamage(Mathf.Max(baseFireballDamage, playerAttack.CurrentFireballDamage - (int)value));
                break;

            case BuffType.HealthRegen:
                // Hentikan regen hanya jika tidak ada buff regen lain
                bool stillHasRegen = activeBuffs.Exists(b => b.data.buffType == BuffType.HealthRegen);
                if (!stillHasRegen && healthRegenCoroutine != null)
                {
                    StopCoroutine(healthRegenCoroutine);
                    healthRegenCoroutine = null;
                }
                break;
        }
    }

    private IEnumerator RegenHealthLoop(float regenPerSecond)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (playerHealth != null && !playerHealth.IsDead)
                playerHealth.AddHealth(regenPerSecond);
        }
    }

    // ─────────────────────────────────────────────
    // UI BUFF ICONS
    // ─────────────────────────────────────────────

    private void SpawnBuffIcon(ActiveBuff buff)
    {
        if (buffIconContainer == null || buffIconPrefab == null) return;

        var icon = Instantiate(buffIconPrefab, buffIconContainer);
        buffIcons[buff] = icon;

        // Set ikon dan warna jika komponen tersedia
        var img = icon.GetComponentInChildren<Image>();
        if (img != null && buff.data.icon != null)
        {
            img.sprite = buff.data.icon;
            img.color = buff.data.buffColor;
        }

        var label = icon.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = buff.isPermanent ? buff.data.flowerName : $"{buff.data.flowerName}\n{buff.remainingTime:F0}s";
    }

    private void UpdateBuffIcon(ActiveBuff buff)
    {
        if (!buffIcons.TryGetValue(buff, out var icon) || icon == null) return;

        var label = icon.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
            label.text = $"{buff.data.flowerName}\n{buff.remainingTime:F0}s";
    }

    private void DestroyBuffIcon(ActiveBuff buff)
    {
        if (buffIcons.TryGetValue(buff, out var icon) && icon != null)
            Destroy(icon);
        buffIcons.Remove(buff);
    }
}
