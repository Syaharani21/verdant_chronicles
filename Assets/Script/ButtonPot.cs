using UnityEngine;

public class ButtonPot : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; 
   private GameObject currentPlant;
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
        bool isEmpty = currentPlant == null;
        Debug.Log($"Pot {name} is {(isEmpty ? "empty" : "occupied")}.");
        return isEmpty;
    }

    public void Plant(GameObject plantPrefab)
    {
        if (IsEmpty())
        {
            GameObject newPlant = Instantiate(plantPrefab, transform);
            newPlant.transform.localPosition = new Vector3(5f, -190f, 0); // Sesuaikan posisi
            newPlant.transform.localScale = Vector3.one;
            newPlant.transform.localRotation = Quaternion.identity;

            currentPlant = newPlant;

            Debug.Log($"Planted {plantPrefab.name} in the pot.");
        }
        else
        {
            Debug.LogWarning("Pot is already occupied.");
        }
    }

    public void RemovePlant()
    {
        if (currentPlant != null)
        {
            Destroy(currentPlant);
            currentPlant = null;
            Debug.Log("Plant removed from the pot.");
            DeactivateBuff();
        }
        else
        {
            Debug.LogWarning("No plant to remove.");
        }
    }

    public void ResetPot()
    {
        RemovePlant(); // Panggil metode RemovePlant untuk menghapus tanaman
    }
   

    private void ActivateBuff()
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
        }
    }


    private void DeactivateBuff()
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
    }
}