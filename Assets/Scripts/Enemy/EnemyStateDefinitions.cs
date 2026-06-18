// ============================================================
//  EnemyStateDefinitions.cs
//  Enum state FSM dan kontainer data adaptasi musuh.
// ============================================================

/// <summary>
/// Lima state utama FSM musuh Verdant Chronicles.
/// </summary>
public enum EnemyState
{
    Patrol,   // Berpatroli di area yang ditentukan
    Chase,    // Mengejar player yang terdeteksi
    Attack,   // Menyerang player dalam jangkauan
    Retreat,  // Mundur ketika HP rendah (adaptif)
    Dead      // Mati
}

/// <summary>
/// Data yang dikumpulkan selama runtime untuk mengadaptasi
/// perilaku musuh terhadap gaya bermain player.
/// </summary>
[System.Serializable]
public class EnemyAdaptiveData
{
    // ── Statistik serangan player ─────────────────────────
    public int   playerHitCount       = 0;   // Berapa kali player mengenai musuh ini
    public float playerDamageDealt    = 0f;  // Total damage yang diterima dari player
    public float lastHitTime          = 0f;  // Waktu terakhir kena hit (Time.time)

    // ── Statistik dodge / dash player ────────────────────
    public int   playerDashCount      = 0;   // Berapa kali player melakukan dash
    public float playerAvgDashInterval= 9999f; // Rata-rata interval antar dash
    private float _lastDashTime       = 0f;

    // ── Level agresivitas (naik saat player sering menyerang) ──
    // 0 = santai, 1 = normal, 2 = agresif, 3 = sangat agresif
    public int aggressionLevel = 0;

    // ── Apakah musuh sudah pernah diserang? ──────────────
    public bool hasBeenHit => playerHitCount > 0;

    public void RegisterPlayerHit(float damage)
    {
        playerHitCount++;
        playerDamageDealt += damage;
        lastHitTime = UnityEngine.Time.time;
        RecalculateAggression();
    }

    public void RegisterPlayerDash()
    {
        float now = UnityEngine.Time.time;
        if (playerDashCount > 0)
        {
            float interval = now - _lastDashTime;
            playerAvgDashInterval = UnityEngine.Mathf.Lerp(playerAvgDashInterval, interval, 0.4f);
        }
        _lastDashTime = now;
        playerDashCount++;
    }

    private void RecalculateAggression()
    {
        if (playerHitCount >= 6)       aggressionLevel = 3;
        else if (playerHitCount >= 4)  aggressionLevel = 2;
        else if (playerHitCount >= 2)  aggressionLevel = 1;
        else                           aggressionLevel = 0;
    }
}
