
using UnityEngine;

public class Pot : MonoBehaviour
{
    [SerializeField] private Transform flowerSlot; // Posisi bunga di pot
    private GameObject plantedFlower; // Referensi bunga yang ditanam

    // Fungsi menanam bunga
    public bool PlantFlower(GameObject flowerPrefab)
    {
        if (plantedFlower != null)
        {
            Debug.LogWarning("Pot already has a flower!");
            return false;
        }

        // Menanam bunga
        plantedFlower = Instantiate(flowerPrefab, flowerSlot.position, Quaternion.identity, flowerSlot);
        Debug.Log($"Flower planted: {flowerPrefab.name} at {flowerSlot.position}");
        return true;
    }

    public bool RemoveFlower()
    {
        // Periksa apakah ada bunga yang ditanam di flowerSlot
        if (flowerSlot.childCount > 0)
        {
            Transform flower = flowerSlot.GetChild(0); // Ambil bunga pertama
            Debug.Log($"Removing flower: {flower.name}");

            Destroy(flower.gameObject); // Hapus bunga
            plantedFlower = null;
            Debug.Log("Flower removed successfully.");
            return true;
        }
        else
        {
            Debug.LogWarning("No flower to remove in the slot!");
            return false;
        }
    }

    // Ketika pot diklik
    private void OnMouseDown()
{
    if (Input.GetMouseButtonDown(0)) // Klik kiri untuk menanam
    {
        Debug.Log("Left mouse button clicked.");
        PlantManager plantManager = FindObjectOfType<PlantManager>();
        if (plantManager != null && plantedFlower == null)
        {
            plantManager.OpenInventory(this);
        }
    }
    else if (Input.GetMouseButtonDown(1)) // Klik kanan untuk menghapus bunga
    {
        Debug.Log("Right mouse button clicked.");
        RemoveFlower();
    }
}

}
