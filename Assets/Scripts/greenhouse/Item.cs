using UnityEngine;

[CreateAssetMenu(fileName = "New Flower", menuName = "Inventory/Flower")]
public class Flower : ScriptableObject
{
    public string flowerName;  // Nama bunga
    public Sprite flowerSprite;  // Gambar bunga
    public float damageMultiplier;  // Buff damage
    public float speedBoost;  // Buff kecepatan
    public float healthMultiplier;  // Buff kesehatan
}
