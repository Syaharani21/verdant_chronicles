using UnityEngine;
using UnityEngine.UI;

public class UIWeaponSwitcher : MonoBehaviour
{
    [Header("Weapon UI")]
    [SerializeField] private Image rangedWeaponIcon; // Ikon untuk senjata panah
    [SerializeField] private Image meleeWeaponIcon;  // Ikon untuk senjata melee

    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.white; // Warna terang untuk senjata aktif
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.5f); // Warna gelap untuk senjata tidak aktif

    private bool isMeleeAttack = false; // Default senjata (sama seperti skrip attack Anda)

    private void Start()
    {
        UpdateWeaponUI(); // Atur status awal UI
    }

    private void Update()
    {
        // Deteksi pergantian senjata (mengikuti logika skrip PlayerAttack Anda)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isMeleeAttack = false; // Aktifkan senjata jarak jauh
            UpdateWeaponUI();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isMeleeAttack = true; // Aktifkan senjata melee
            UpdateWeaponUI();
        }
    }

    private void UpdateWeaponUI()
    {
        // Jika senjata melee aktif
        if (isMeleeAttack)
        {
            meleeWeaponIcon.color = activeColor; // Terang
            rangedWeaponIcon.color = inactiveColor; // Gelap
        }
        else // Jika senjata panah aktif
        {
            meleeWeaponIcon.color = inactiveColor; // Gelap
            rangedWeaponIcon.color = activeColor; // Terang
        }
    }
}
