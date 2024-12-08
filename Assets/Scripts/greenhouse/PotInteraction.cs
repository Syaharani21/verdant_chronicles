using UnityEngine;

public class PotButton : MonoBehaviour
{
    public GameObject inventoryPanel; // Panel Inventory yang akan ditampilkan/disembunyikan

    private bool isInventoryVisible = false; // Status visibilitas inventory

    // Fungsi yang akan dipanggil saat tombol ditekan
    public void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible; // Ganti status visibilitas
        inventoryPanel.SetActive(isInventoryVisible); // Tampilkan atau sembunyikan panel
    }
}
