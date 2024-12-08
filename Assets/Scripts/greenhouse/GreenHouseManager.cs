using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public void OnClick()
    {
        inventoryManager.OnItemClicked(gameObject);
    }
}
