using UnityEngine;

/// <summary>
/// Tipe buff yang bisa diberikan oleh bunga.
/// </summary>
public enum BuffType
{
    SpeedBoost,       // Meningkatkan kecepatan gerak
    JumpBoost,        // Meningkatkan tinggi lompatan
    HealthRegen,      // Regenerasi HP secara berkala
    AttackBoost,      // Meningkatkan damage serangan
    DashCooldownReduction // Mengurangi cooldown dash
}

/// <summary>
/// ScriptableObject yang mendefinisikan data bunga dan buff yang diberikan.
/// Buat asset baru via: klik kanan di Project > Create > Verdant Chronicles > Flower Data
/// </summary>
[CreateAssetMenu(fileName = "NewFlowerData", menuName = "Verdant Chronicles/Flower Data")]
public class FlowerData : ScriptableObject
{
    [Header("Identitas Bunga")]
    public string flowerName;           // Nama tampilan bunga
    [TextArea] public string description; // Deskripsi singkat buff
    public Sprite icon;                 // Ikon untuk UI inventory

    [Header("Buff")]
    public BuffType buffType;           // Jenis buff
    public float buffValue;             // Nilai buff (misal: 2 untuk +2 speed)
    public float buffDuration;          // Durasi buff dalam detik (0 = permanen selama di pot)

    [Header("Visual")]
    public Color buffColor = Color.green; // Warna indikator buff di UI
}
