
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Flower[] flowers; // Array bunga
    public GameObject inventoryPanel; // Panel Inventory
    public GameObject inventorySlotPrefab; // Prefab slot inventory

    private void Start()
    {
        if (inventoryPanel == null || inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory Panel atau Inventory Slot Prefab belum diatur.");
            return;
        }
    }

    public void OpenInventory(GameObject pot)
    {
        if (pot == null)
        {
            Debug.LogError("Pot tidak valid.");
            return;
        }

        inventoryPanel.SetActive(true);
        Debug.Log("Inventory dibuka.");

        // Hapus slot lama
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Tambahkan slot untuk bunga
        foreach (Flower flower in flowers)
        {
            if (flower == null)
            {
                Debug.LogWarning("Flower tidak ada.");
                continue;
            }

            // Instansiasi slot inventory
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);

            // Atur gambar bunga
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = flower.flowerSprite;
            }
            else
            {
                Debug.LogError("Slot tidak memiliki komponen Image!");
            }

            // Tambahkan event tombol
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(() => PlantFlower(pot, flower));
            }
            else
            {
                Debug.LogError("Slot tidak memiliki komponen Button!");
            }
        }
    }

    public void PlantFlower(GameObject pot, Flower flower)
    {
        if (pot == null || flower == null)
        {
            Debug.LogError("Pot atau bunga tidak valid.");
            return;
        }

        Debug.Log($"Menanam bunga {flower.name} di pot {pot.name}.");
        pot.GetComponent<Pot>().PlantFlower(flower);
        inventoryPanel.SetActive(false);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("Inventory ditutup.");
        }
        else
        {
            Debug.LogError("Inventory Panel tidak diatur.");
        }
    }
}
