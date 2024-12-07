using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Flower[] flowers; // Array bunga yang tersedia
    public GameObject inventoryPanel; // Panel Inventory
    public GameObject pot; // Referensi ke pot saat ini

    void Start()
    {
        PopulateInventory();
    }

    void PopulateInventory()
    {
        // Ambil semua Button di dalam Inventory Panel
        foreach (Transform child in inventoryPanel.transform)
        {
            Button flowerButton = child.GetComponent<Button>();
            int index = child.GetSiblingIndex();

            if (index < flowers.Length)
            {
                // Set gambar tombol bunga
                Image buttonImage = flowerButton.GetComponent<Image>();
                buttonImage.sprite = flowers[index].flowerSprite;

                // Hapus semua listener untuk mencegah duplikasi
                flowerButton.onClick.RemoveAllListeners();

                // Tambahkan event untuk tombol
                flowerButton.onClick.AddListener(() => PlantFlower(flowers[index]));
            }
        }
    }

    public void OpenInventory(GameObject selectedPot)
    {
        pot = selectedPot;
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    void PlantFlower(Flower flower)
    {
        if (pot != null)
        {
            pot.GetComponent<Pot>().PlantFlower(flower);
            CloseInventory();
        }
    }
}
