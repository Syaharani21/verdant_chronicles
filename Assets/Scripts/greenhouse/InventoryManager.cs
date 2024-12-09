using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public GameObject inventoryPanel; // Panel inventory
    public GameObject[] flowerPrefabs; // Prefab bunga
    private Pot selectedPot; // Pot yang dipilih untuk ditanam bunga

    private void Start()
    {
        // Pastikan inventory tidak aktif saat awal
        inventoryPanel.SetActive(false);
    }

    // Method untuk membuka inventory saat pot diklik
    public void OpenInventory(Pot pot)
    {
        selectedPot = pot; // Simpan pot yang diklik
        inventoryPanel.SetActive(true); // Tampilkan inventory
        Debug.Log("Inventory opened for pot: " + pot.name);
    }

    // Method untuk menanam bunga di pot dari inventory
    public void PlantFlower(string flowerName)
    {
        if (selectedPot == null)
        {
            Debug.LogWarning("No pot selected!");
            return;
        }

        // Cari prefab bunga berdasarkan nama
        GameObject flowerPrefab = System.Array.Find(flowerPrefabs, f => f.name == flowerName);
        if (flowerPrefab != null)
        {
            if (selectedPot.PlantFlower(flowerPrefab))
            {
                Debug.Log("Flower planted: " + flowerName + " in pot: " + selectedPot.name);
            }
        }
        else
        {
            Debug.LogWarning("Flower prefab not found: " + flowerName);
        }

        // Tutup inventory setelah menanam bunga
        CloseInventory();
    }

    // Method untuk menutup inventory
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        selectedPot = null;
        Debug.Log("Inventory closed");
    }
}