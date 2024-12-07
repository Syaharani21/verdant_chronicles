using UnityEngine;

public class ButtonPot : MonoBehaviour
{
     [SerializeField] private GameObject flowerPrefab; // Prefab bunga
    [SerializeField] private Transform flowerSpawnPoint; // Posisi bunga muncul
    [SerializeField] private float growTime = 10f; // Waktu tumbuh bunga
    [SerializeField] private string buffType = "damage"; // Buff dari bunga
    [SerializeField] private float buffValue = 1.5f; // Nilai buff

    private bool isPlanted = false;
    private bool isGrown = false;
    private string plantedFlowerType; // Jenis bunga yang ditanam
    private GameObject currentFlower; // Referensi bunga di pot

    private void OnMouseDown()
    {
        if (!isPlanted)
        {
            // Buka inventory jika belum ada bunga
            FindObjectOfType<InventoryUi>().OpenInventory(this);
        }
        else if (isGrown)
        {
            // Panen bunga jika sudah tumbuh
            HarvestFlower();
        }
    }

    public void PlantFlower(string flowerType)
    {
        if (!isPlanted)
        {
            plantedFlowerType = flowerType;
            isPlanted = true;
            Debug.Log($"Planted {flowerType}!");
            StartCoroutine(GrowFlower());
        }
    }

    private System.Collections.IEnumerator GrowFlower()
    {
        yield return new WaitForSeconds(growTime);
        isGrown = true;
        Debug.Log("Flower fully grown!");

        // Tampilkan bunga di pot
        if (flowerPrefab != null && flowerSpawnPoint != null)
        {
            currentFlower = Instantiate(flowerPrefab, flowerSpawnPoint.position, Quaternion.identity, transform);
        }
    }

    private void HarvestFlower()
    {
        if (isGrown)
        {
            Debug.Log($"Harvested {plantedFlowerType}! Applying buff...");

            // Berikan buff ke pemain
            ApplyBuff();

            // Reset pot
            isPlanted = false;
            isGrown = false;

            // Hapus bunga
            if (currentFlower != null)
            {
                Destroy(currentFlower);
            }
        }
    }

    private void ApplyBuff()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            switch (buffType.ToLower())
            {
                case "damage":
                    playerStats.IncreaseDamage(buffValue);
                    break;
                case "speed":
                    playerStats.IncreaseSpeed(buffValue);
                    break;
                case "health":
                    playerStats.IncreaseHealth(buffValue);
                    break;
                default:
                    Debug.LogWarning("Unknown buff type!");
                    break;
            }
        }
    }
}
