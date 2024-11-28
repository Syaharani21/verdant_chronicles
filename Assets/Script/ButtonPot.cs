using UnityEngine;
using System.Collections.Generic; // Untuk List

public class ButtonPot : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; 
    private List<GameObject> currentPlants = new List<GameObject>(); // Menyimpan banyak tanaman
    private bool isBuffActive = false;

    public enum BuffType { Damage, Speed, Health }
    public BuffType buffType;
    public float buffAmount;

    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);
    }

    public bool IsEmpty()
    {
        bool isEmpty = currentPlants.Count == 0;
        Debug.Log($"Pot {name} is {(isEmpty ? "empty" : "occupied")}.");
        return isEmpty;
    }

    public void Plant(GameObject plantPrefab)
    {
        // Instansiasi tanaman baru
        GameObject newPlant = Instantiate(plantPrefab, transform);

        // Atur posisi tanaman baru secara dinamis berdasarkan jumlah tanaman saat ini
        float offset = currentPlants.Count * 0.5f; // Jarak antar tanaman
        newPlant.transform.localPosition = new Vector3(offset, 0f, -1f); // Posisi relatif
        newPlant.transform.localScale = Vector3.one;
        newPlant.transform.localRotation = Quaternion.identity;

        // Tambahkan tanaman ke daftar
        currentPlants.Add(newPlant);

        Debug.Log($"Planted {plantPrefab.name} in pot {name}. Total plants: {currentPlants.Count}.");

        // Aktifkan buff jika perlu
        if (!isBuffActive)
        {
            ActivateBuff();
        }
    }

    public void RemovePlant(GameObject plant)
    {
        if (currentPlants.Contains(plant))
        {
            currentPlants.Remove(plant);
            Destroy(plant);
            Debug.Log($"Removed {plant.name} from pot {name}. Total plants: {currentPlants.Count}.");

            // Nonaktifkan buff jika tidak ada tanaman
            if (currentPlants.Count == 0)
            {
                DeactivateBuff();
            }
        }
        else
        {
            Debug.LogWarning("The specified plant is not in this pot.");
        }
    }

    public void RemoveAllPlants()
    {
        foreach (var plant in currentPlants)
        {
            Destroy(plant);
        }
        currentPlants.Clear();

        Debug.Log($"All plants removed from pot {name}.");

        // Nonaktifkan buff
        DeactivateBuff();
    }

    public void ActivateBuff()
    {
        if (!isBuffActive)
        {
            isBuffActive = true;
            switch (buffType)
            {
                case BuffType.Damage:
                    PlayerStats.instance.damageMultiplier *= buffAmount;
                    break;
                case BuffType.Speed:
                    PlayerStats.instance.speedMultiplier *= buffAmount;
                    break;
                case BuffType.Health:
                    PlayerStats.instance.healthMultiplier *= buffAmount;
                    break;
            }

            Debug.Log($"Buff {buffType} activated with amount {buffAmount}.");
        }
    }

    private void DeactivateBuff()
    {
        if (isBuffActive)
        {
            isBuffActive = false;
            switch (buffType)
            {
                case BuffType.Damage:
                    PlayerStats.instance.damageMultiplier /= buffAmount;
                    break;
                case BuffType.Speed:
                    PlayerStats.instance.speedMultiplier /= buffAmount;
                    break;
                case BuffType.Health:
                    PlayerStats.instance.healthMultiplier /= buffAmount;
                    break;
            }

            Debug.Log($"Buff {buffType} deactivated.");
        }
    }
}
