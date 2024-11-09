using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private HashSet<Item> items = new HashSet<Item>();

    public void AddItem(Item item)
    {
        if (items.Add(item))
        {
            Debug.Log($"Item added: {item.itemName}");
        }
        else
        {
            Debug.Log($"Item already exists: {item.itemName}");
        }
    }

    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"Item removed: {item.itemName}");
        }
        else
        {
            Debug.Log($"Item not found: {item.itemName}");
        }
    }

    public void ShowInventory()
    {
        if (items.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        foreach (var item in items)
        {
            Debug.Log($"Inventory Item: {item.itemName}");
        }
    }

    public int GetItemCount()
    {
        return items.Count;
    }
}