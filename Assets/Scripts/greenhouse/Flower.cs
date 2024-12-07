using UnityEngine;

[CreateAssetMenu(fileName = "Flower", menuName = "Inventory/Flower")]
public class Flower : ScriptableObject
{
    public string flowerName;
    public Sprite flowerSprite;
    public float damageMultiplier;
    public float speedBoost;
    public float healthMultiplier;
}
