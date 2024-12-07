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
    Debug.Log("Populating Inventory...");

    // Hapus semua item lama
    foreach (Transform child in inventoryContent)
    {
        Destroy(child.gameObject);
    }

    // Tambahkan bunga ke inventory UI
    foreach (string flower in InventoryManager.Instance.flowers)
{
    Debug.Log($"Instantiating item for: {flower}");
    GameObject item = Instantiate(inventoryItemPrefab, inventoryContent);

    Debug.Log($"Created item: {item.name}, Parent: {item.transform.parent.name}");
    // Tambahkan log untuk elemen teks
    TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
    
    if (inventoryItemPrefab == null)
{
    Debug.LogError("inventoryItemPrefab is null!");
    Debug.Break(); // Berhenti saat runtime untuk memeriksa status
}
    if (itemText != null)
    {
        itemText.text = flower;
        Debug.Log($"Set text for item: {flower}");
    }
    else
    {
        Debug.LogError("TMP_Text not found in prefab!");
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
