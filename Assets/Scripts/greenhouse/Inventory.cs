using UnityEngine;

public class PotButton : MonoBehaviour
{
    public GameObject ButtonPot;

    public void OnPotClick()
    {
        FindObjectOfType<InventoryManager>().OpenInventory(ButtonPot);
    }
}