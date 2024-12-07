using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Flower[] flowers;
    public GameObject inventoryPanel;
    public GameObject potPrefab; // Prefab pot dengan mekanisme tanam

    public void OpenInventory(GameObject pot)
    {
        inventoryPanel.SetActive(true);

        // Bind buttons in inventory with flower planting
        foreach (Transform child in inventoryPanel.transform)
        {
            Button flowerButton = child.GetComponent<Button>();
            int index = child.GetSiblingIndex();
            if (index < flowers.Length)
            {
                flowerButton.onClick.AddListener(() => PlantFlower(pot, flowers[index]));
            }
        }
    }

    public void PlantFlower(GameObject pot, Flower flower)
    {
        pot.GetComponent<Pot>().PlantFlower(flower);
        inventoryPanel.SetActive(false);
    }
}
