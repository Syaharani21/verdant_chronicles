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