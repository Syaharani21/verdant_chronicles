using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public int quantity = 1; 
    public GameObject inventoryPanel; 

    public GameObject InventorySlot;
    public void UseItem()
    {
        if (quantity > 0)
        {
            quantity--; 

            if (quantity == 0)
            {
                
                InventorySlot.SetActive(false);
            }
        }
    }
   
    public void exit()
    {
        inventoryPanel.SetActive(false); 
   
    }
}
