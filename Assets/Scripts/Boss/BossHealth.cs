using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 300;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private Animator anim;

    private int currentHealth;
    private bool die = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (die) return;

        currentHealth -= damage;

        if (hurtSound != null)
            SoundManager.instance.PlaySound(hurtSound);

        anim.SetTrigger("hurt"); // Trigger animasi hurt

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        die = true;
        anim.SetTrigger("die"); // Trigger animasi die

        if (deathSound != null)
            SoundManager.instance.PlaySound(deathSound);

        GetComponent<BossBehavior>().enabled = false;
        Destroy(gameObject, 2f); // Hapus game object setelah 2 detik
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
