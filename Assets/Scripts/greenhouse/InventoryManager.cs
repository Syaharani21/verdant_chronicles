using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mengelola panel inventory greenhouse.
/// Setiap bunga di inventory diwakili oleh FlowerData (ScriptableObject).
/// 
/// Setup di Inspector:
/// - inventoryPanel : Panel UI yang menampilkan pilihan bunga
/// - flowerEntries  : Array pasangan (FlowerData, prefab GameObject)
/// </summary>
public class PlantManager : MonoBehaviour
{
    [System.Serializable]
    public struct FlowerEntry
    {
        public FlowerData data;         // ScriptableObject data bunga
        public GameObject prefab;       // Prefab visual bunga untuk di-spawn di pot
    }

    [Header("UI")]
    public GameObject inventoryPanel;   // Panel pilihan bunga

    [Header("Bunga yang Tersedia")]
    public FlowerEntry[] flowerEntries; // Daftar bunga yang bisa ditanam

    [Header("Tombol Bunga (Opsional Auto-Generate)")]
    [SerializeField] private Transform buttonContainer;  // Parent untuk tombol bunga
    [SerializeField] private GameObject flowerButtonPrefab; // Prefab tombol (Button + Image + TMP)

    private Pot selectedPot;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        GenerateFlowerButtons();
    }

    // ─────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Buka inventory untuk pot yang diklik.
    /// </summary>
    public void OpenInventory(Pot pot)
    {
        selectedPot = pot;
        inventoryPanel.SetActive(true);
        Debug.Log("Inventory opened for pot: " + pot.name);
    }

    /// <summary>
    /// Tanam bunga berdasarkan nama FlowerData. Dipanggil oleh tombol UI.
    /// </summary>
    public void PlantFlower(string flowerName)
    {
        if (selectedPot == null)
        {
            Debug.LogWarning("No pot selected!");
            return;
        }

        // Cari entry berdasarkan nama
        FlowerEntry entry = System.Array.Find(flowerEntries, f => f.data != null && f.data.flowerName == flowerName);
        if (entry.data != null && entry.prefab != null)
        {
            selectedPot.PlantFlower(entry.prefab, entry.data);
            Debug.Log($"Planted {flowerName} in {selectedPot.name}");
        }
        else
        {
            Debug.LogWarning($"Flower not found or missing prefab: {flowerName}");
        }

        CloseInventory();
    }

    /// <summary>
    /// Tutup panel inventory.
    /// </summary>
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        selectedPot = null;
        Debug.Log("Inventory closed");
    }

    // ─────────────────────────────────────────────
    // TOMBOL OTOMATIS
    // ─────────────────────────────────────────────

    /// <summary>
    /// Membuat tombol otomatis untuk setiap bunga (opsional, perlu prefab tombol).
    /// </summary>
    private void GenerateFlowerButtons()
    {
        if (buttonContainer == null || flowerButtonPrefab == null) return;

        // Hapus tombol lama
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (var entry in flowerEntries)
        {
            if (entry.data == null) continue;

            var btnObj = Instantiate(flowerButtonPrefab, buttonContainer);

            // Set ikon
            var img = btnObj.GetComponentInChildren<Image>();
            if (img != null && entry.data.icon != null)
            {
                img.sprite = entry.data.icon;
                img.color = entry.data.buffColor;
            }

            // Set label
            var label = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = $"{entry.data.flowerName}\n<size=10>{entry.data.description}</size>";

            // Sambungkan ke PlantFlower
            var btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                string capturedName = entry.data.flowerName; // Capture untuk lambda
                btn.onClick.AddListener(() => PlantFlower(capturedName));
            }
        }
    }
}
