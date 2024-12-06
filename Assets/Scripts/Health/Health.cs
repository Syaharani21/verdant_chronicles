using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float StartingHealth => startingHealth;
    public float currentHealth { get; private set; }

    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components to Disable on Death")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip deathSound1;
    [SerializeField] private AudioClip deathSound2;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public bool IsDead
    {
        get { return dead; }
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            if (!dead)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        dead = true;
        anim.SetTrigger("die");
        SoundManager.instance.PlaySound(deathSound1);
        SoundManager.instance.PlaySound(deathSound2);

        foreach (Behaviour component in components)
        {
            if (component != null) 
                component.enabled = false;
        }
    }

    public void AddHealth(float _value)
    {
        if (!dead) 
        {
            currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        }
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
