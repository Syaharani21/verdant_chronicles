using UnityEngine;

public class PotButton : MonoBehaviour
{
    public GameObject pot;

    public void OnPotClick()
    {
        FindObjectOfType<InventoryManager>().OpenInventory(pot);
    }
}
