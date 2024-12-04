using System.Collections.Generic;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel; // Panel inventory

    [Header("Planting Options")]
    [SerializeField] private List<GameObject> plantPrefabs; // Daftar prefab bunga
    [SerializeField] private List<int> plantQuantities; // Jumlah bunga di inventory

    [Header("Pots in Greenhouse")]
    [SerializeField] private List<ButtonPot> potButtons; // Daftar pot di greenhouse

    private int selectedPlantIndex = -1; // Index bunga yang dipilih

    private void Start()
    {
        HideInventory();
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Inventory panel is not assigned.");
        }
    }

    private void HideInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventory panel is not assigned.");
        }
    }

    public void SelectPlant(int index)
    {
        if (index >= 0 && index < plantPrefabs.Count && plantQuantities[index] > 0)
        {
            selectedPlantIndex = index;
            Debug.Log($"Selected plant: {plantPrefabs[index].name}");
        }
        else
        {
            Debug.LogWarning("Invalid plant selection or out of stock.");
        }
    }

    public void PlantInPot(ButtonPot potButton)
    {
        if (selectedPlantIndex >= 0 && potButton.IsEmpty() && plantQuantities[selectedPlantIndex] > 0)
        {
            potButton.Plant(plantPrefabs[selectedPlantIndex]);
            plantQuantities[selectedPlantIndex]--;
            Debug.Log($"Planted {plantPrefabs[selectedPlantIndex].name} in pot.");
        }
        else
        {
            Debug.LogWarning("Unable to plant: Invalid selection or pot is occupied.");
        }
    }
}
