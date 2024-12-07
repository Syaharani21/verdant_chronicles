using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton untuk akses global
    public List<string> flowers = new List<string>(); // List untuk menyimpan bunga

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tambahkan bunga ke inventory untuk testing
        AddFlower("Bunga");
        AddFlower("Bunga");
        AddFlower("Bunga");
        AddFlower("Bunga");
        AddFlower("Bunga");
    }

    public void AddFlower(string flowerName)
    {
        flowers.Add(flowerName);
        Debug.Log($"Added {flowerName} to inventory!");
    }

    public void RemoveFlower(string flowerName)
    {
        if (flowers.Contains(flowerName))
        {
            flowers.Remove(flowerName);
            Debug.Log($"Removed {flowerName} from inventory!");
        }
    }

    public bool HasFlower(string flowerName)
    {
        return flowers.Contains(flowerName);
    }
}
