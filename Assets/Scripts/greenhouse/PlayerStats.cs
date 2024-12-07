using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float baseDamage = 10f;
    public float baseSpeed = 5f;
    public float baseHealth = 100f;

    private float currentDamage;
    private float currentSpeed;
    private float currentHealth;

    private void Awake()
    {
        Instance = this;
        ResetStats();
    }

    public void ApplyBuff(Flower flower)
    {
        currentDamage *= flower.damageMultiplier > 0 ? flower.damageMultiplier : 1;
        currentSpeed += flower.speedBoost;
        currentHealth *= flower.healthMultiplier > 0 ? flower.healthMultiplier : 1;
        UpdateUI();
    }

    public void RemoveBuff(Flower flower)
    {
        ResetStats(); // Optional: Recalculate based on other active buffs
    }

    private void ResetStats()
    {
        currentDamage = baseDamage;
        currentSpeed = baseSpeed;
        currentHealth = baseHealth;
    }

    private void UpdateUI()
    {
        // Update player UI stats (e.g., HP, Damage, Speed)
    }
}
