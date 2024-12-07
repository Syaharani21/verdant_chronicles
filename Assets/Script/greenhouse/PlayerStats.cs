using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    private float baseDamage = 10f;
    private float baseSpeed = 5f;
    private float baseHealth = 100f;

    public void IncreaseDamage(float multiplier)
    {
        baseDamage *= multiplier;
        Debug.Log($"Damage increased! New Damage: {baseDamage}");
    }

    public void IncreaseSpeed(float multiplier)
    {
        baseSpeed *= multiplier;
        Debug.Log($"Speed increased! New Speed: {baseSpeed}");
    }

    public void IncreaseHealth(float multiplier)
    {
        baseHealth *= multiplier;
        Debug.Log($"Health increased! New Health: {baseHealth}");
    }
}