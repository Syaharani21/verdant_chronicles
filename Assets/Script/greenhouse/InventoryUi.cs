using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Untuk Button dan UI Text
using TMPro; // Untuk TextMeshPro

public class InventoryUi : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel inventory
    [SerializeField] private Transform inventoryContent; // Tempat item ditampilkan
    [SerializeField] private GameObject inventoryItemPrefab; // Prefab untuk item bunga

    private ButtonPot currentPot; // Pot yang sedang dipilih

    private void Start()
    {
        inventoryPanel.SetActive(false); // Sembunyikan panel di awal
        InventoryManager.Instance.AddFlower("Rose");
        InventoryManager.Instance.AddFlower("Sunflower");
        InventoryManager.Instance.AddFlower("Tulip");
    }

    public void OpenInventory(ButtonPot pot)
    {
        currentPot = pot;
        inventoryPanel.SetActive(true);
        PopulateInventory();
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    private void PopulateInventory()
    {
        // Hapus item lama
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // Tambahkan item bunga ke inventory UI
        foreach (string flower in InventoryManager.Instance.flowers)
        {
            GameObject item = Instantiate(inventoryItemPrefab, inventoryContent);
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
            if (itemText != null)
            {
                itemText.text = flower;
            }

            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectFlower(flower));
            }
        }
    }

    private void SelectFlower(string flower)
    {
        if (currentPot != null)
        {
            currentPot.PlantFlower(flower);
        }

        InventoryManager.Instance.RemoveFlower(flower); // Hapus bunga dari inventory
        CloseInventory();
    }
}
