using UnityEngine;

public class Pot : MonoBehaviour
{
    public Flower plantedFlower;
    public GameObject flowerSpriteRenderer;

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
        Destroy(flowerSpriteRenderer);
        plantedFlower = null;
    }

    private void RemoveBuff()
    {
        PlayerStats.Instance.RemoveBuff(plantedFlower);
    }
}
