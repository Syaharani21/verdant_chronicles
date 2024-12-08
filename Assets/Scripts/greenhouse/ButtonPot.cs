using UnityEngine;

public class Pot : MonoBehaviour
{
    public Flower plantedFlower;  // Bunga yang ditanam
    public GameObject flowerSpriteRenderer;  // Renderer untuk menampilkan bunga

    public void PlantFlower(Flower flower)
    {
        plantedFlower = flower;
        flowerSpriteRenderer.GetComponent<SpriteRenderer>().sprite = flower.flowerSprite;
        ApplyBuff(flower);
    }

    private void ApplyBuff(Flower flower)
    {
        PlayerStats.Instance.ApplyBuff(flower);
    }

    public void HarvestFlower()
    {
        RemoveBuff();
        Destroy(flowerSpriteRenderer);  // Hapus bunga dari pot
        plantedFlower = null;
    }

    private void RemoveBuff()
    {
        PlayerStats.Instance.RemoveBuff(plantedFlower);
    }
}
