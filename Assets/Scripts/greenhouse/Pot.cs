using UnityEngine;

/// <summary>
/// Mengatur satu pot tanaman di greenhouse.
/// - Klik kiri  : buka inventory untuk menanam bunga (jika pot kosong)
/// - Klik kanan : cabut bunga yang sudah ditanam (jika ada)
/// 
/// Saat bunga ditanam  → memanggil PlayerBuffSystem.ApplyBuff()
/// Saat bunga dicabut  → memanggil PlayerBuffSystem.RemoveBuff()
/// </summary>
public class Pot : MonoBehaviour
{
    [SerializeField] private Transform flowerSlot;   // Posisi bunga di pot
    private GameObject plantedFlower;                 // Referensi bunga yang ditanam
    private FlowerData currentFlowerData;             // Data bunga (untuk sistem buff)

    // ─────────────────────────────────────────────
    // PUBLIC API — dipanggil oleh PlantManager
    // ─────────────────────────────────────────────

    /// <summary>
    /// Tanam bunga di pot ini. Menerapkan buff ke player.
    /// </summary>
    public bool PlantFlower(GameObject flowerPrefab, FlowerData flowerData)
    {
        if (plantedFlower != null)
        {
            Debug.LogWarning("Pot already has a flower!");
            return false;
        }

        // Spawn visual bunga
        plantedFlower = Instantiate(flowerPrefab, flowerSlot.position, Quaternion.identity, flowerSlot);
        currentFlowerData = flowerData;

        // Terapkan buff ke player
        if (flowerData != null && PlayerBuffSystem.instance != null)
        {
            PlayerBuffSystem.instance.ApplyBuff(flowerData, isPermanent: true);
            Debug.Log($"[Pot] Bunga {flowerData.flowerName} ditanam → buff {flowerData.buffType} +{flowerData.buffValue} aktif.");
        }

        Debug.Log($"Flower planted: {flowerPrefab.name} at {flowerSlot.position}");
        return true;
    }

    /// <summary>
    /// Cabut bunga dari pot. Menghapus buff dari player.
    /// </summary>
    public bool RemoveFlower()
    {
        if (flowerSlot.childCount == 0)
        {
            Debug.LogWarning("No flower to remove in the slot!");
            return false;
        }

        // Hapus buff dari player sebelum menghancurkan bunga
        if (currentFlowerData != null && PlayerBuffSystem.instance != null)
        {
            PlayerBuffSystem.instance.RemoveBuff(currentFlowerData);
            Debug.Log($"[Pot] Bunga {currentFlowerData.flowerName} dicabut → buff dihapus.");
        }

        // Hancurkan visual bunga
        Transform flower = flowerSlot.GetChild(0);
        Destroy(flower.gameObject);
        plantedFlower = null;
        currentFlowerData = null;

        Debug.Log("Flower removed successfully.");
        return true;
    }

    /// <summary>
    /// Apakah pot ini sudah terisi?
    /// </summary>
    public bool HasFlower => plantedFlower != null;

    /// <summary>
    /// Data bunga yang sedang ditanam (null jika kosong).
    /// </summary>
    public FlowerData CurrentFlowerData => currentFlowerData;

    // ─────────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────────

    private void OnMouseDown()
    {
        // Klik kiri: buka inventory jika pot kosong
        Debug.Log("Pot clicked (left).");
        if (plantedFlower == null)
        {
            PlantManager plantManager = FindObjectOfType<PlantManager>();
            if (plantManager != null)
                plantManager.OpenInventory(this);
        }
    }

    private void OnMouseOver()
    {
        // Klik kanan: cabut bunga jika ada
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Pot right-clicked.");
            RemoveFlower();
        }
    }
}
