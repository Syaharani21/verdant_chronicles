using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log("Item added: " + item.itemName);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        Debug.Log("Item removed: " + item.itemName);
    }

    public void ShowInventory()
    {
        foreach (var item in items)
        {
            Debug.Log("Inventory Item: " + item.itemName);
        }
    }
}