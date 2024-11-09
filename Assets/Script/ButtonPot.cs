using UnityEngine;

public class ButtonPot : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; 

    public void ShowInventory()
    {
        
        inventoryPanel.SetActive(true);
    }
}