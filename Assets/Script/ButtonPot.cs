using UnityEngine;
using System.Collections.Generic; // Untuk List

public class ButtonPot : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private float growthTime = 10f; // Waktu pertumbuhan tanaman (dalam detik)
    [SerializeField] private BuffType buffType;
    [SerializeField] private float buffAmount = 1.5f;

    private List<GameObject> currentPlants = new List<GameObject>(); // Menyimpan banyak tanaman
    private bool isBuffActive = false;

    // BuffType: Jenis buff yang diaktifkan oleh pot
    public enum BuffType { Damage, Speed, Health }

    // Fungsi untuk menampilkan inventory
    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);
    }

    // Fungsi untuk mengecek apakah pot kosong
    public bool IsEmpty()
    {
        return currentPlants.Count == 0;
    }

    // Fungsi untuk menanam tanaman di pot
    public void Plant(GameObject plantPrefab)
    {
        if (currentPlants.Count > 0)
        {
            Debug.LogWarning("Pot already has a plant. Please remove it before planting a new one.");
            return;
        }

        // Instansiasi tanaman baru
        GameObject newPlant = Instantiate(plantPrefab, transform);

        // Set posisi tanaman agar sesuai dengan posisi pot
        newPlant.transform.SetParent(transform);
        newPlant.transform.localPosition = Vector3.zero; // Posisi default
        newPlant.transform.localScale = Vector3.one;     // Reset skala
        newPlant.transform.localRotation = Quaternion.identity;

        // Tambahkan tanaman ke daftar
        currentPlants.Add(newPlant);

        Debug.Log($"Planted {plantPrefab.name} in pot {name}.");

        // Aktifkan pertumbuhan tanaman
        StartCoroutine(GrowPlant(newPlant));
    }

    // Coroutine untuk pertumbuhan tanaman
    private IEnumerator<System.Collections.IEnumerator> GrowPlant(GameObject plant)
    {
        float timer = 0f;

        while (timer < growthTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Tandai tanaman siap dipanen
        if (plant != null)
        {
            Debug.Log($"{plant.name} is ready to harvest!");
            plant.GetComponent<Renderer>().material.color = Color.green; // Ubah warna (contoh)
        }
    }

    // Fungsi untuk memanen tanaman
    // Fungsi untuk memanen tanaman
public void Harvest()
{
    if (currentPlants.Count > 0)
    {
        GameObject plantToHarvest = currentPlants[0];

        // Tambahkan tanaman ke inventory pemain
        InventoryManager.instance.AddToInventory(plantToHarvest); // Kirim objek GameObject, bukan nama

        // Hapus tanaman dari pot
        currentPlants.Remove(plantToHarvest);
        Destroy(plantToHarvest);

        Debug.Log($"Harvested plant from pot {name}.");

        // Nonaktifkan buff jika tidak ada tanaman
        DeactivateBuff();
    }
    else
    {
        Debug.LogWarning("No plants to harvest in this pot.");
    }
}


    // Fungsi untuk mengaktifkan buff
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

            Debug.Log($"Buff {buffType} activated.");
        }
    }

    // Fungsi untuk menonaktifkan buff
    public void DeactivateBuff()
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
