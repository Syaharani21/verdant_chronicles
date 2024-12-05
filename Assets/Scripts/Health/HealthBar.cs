using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        totalhealthBar.fillAmount = playerHealth.currentHealth / playerHealth.StartingHealth;
        UpdateHealthText();
    }

    private void Update()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / playerHealth.StartingHealth;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{playerHealth.currentHealth}/{playerHealth.StartingHealth}";
        }
    }
}
