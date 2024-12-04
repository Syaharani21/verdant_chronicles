using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // List untuk menyimpan objek tanaman di inventory
    private List<GameObject> inventory = new List<GameObject>();

    // Fungsi untuk menambahkan tanaman ke inventory
    public void AddToInventory(GameObject plant)
    {
        // Tambahkan tanaman ke daftar inventory
        inventory.Add(plant);
        Debug.Log($"{plant.name} added to inventory.");
    }

    // Fungsi untuk mendapatkan isi inventory
    public List<GameObject> GetInventory()
    {
        return inventory;
    }
}
