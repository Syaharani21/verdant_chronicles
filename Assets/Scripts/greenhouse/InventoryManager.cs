<<<<<<< HEAD
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public GameObject potObject; // referensi ke pot di scene
    public GameObject InventoryPanel; // referensi ke panel inventory

    public void AddItem(GameObject item)
    {
        if (!inventoryItems.Contains(item))
        {
            inventoryItems.Add(item);
            UpdateInventoryUI();
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            UpdateInventoryUI();
        }
    }

    private void UpdateInventoryUI()
    {
        // Update UI sesuai dengan isi inventoryItems
        // Anda bisa mengatur button text/image untuk mencerminkan item di inventory
        foreach (Transform child in InventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject item in inventoryItems)
        {
            GameObject itemButton = Instantiate(Resources.Load<GameObject>("ItemButton"));
            itemButton.transform.SetParent(InventoryPanel.transform);
            itemButton.GetComponentInChildren<Text>().text = item.name;
            itemButton.GetComponent<Button>().onClick.AddListener(() => OnItemClicked(item));
        }
    }

    public void OnItemClicked(GameObject item)
    {
        RemoveItem(item);
        PlaceItemOnPot(item);
    }

    private void PlaceItemOnPot(GameObject item)
    {
        // Posisikan item di atas pot
        item.transform.position = potObject.transform.position + Vector3.up * 0.5f;
        item.SetActive(true); // munculkan item di atas pot
    }

    public void QuitPanel()
    {
        InventoryPanel.SetActive(false);
    }
}
=======
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
>>>>>>> greenhouse
