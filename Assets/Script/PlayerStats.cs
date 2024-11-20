using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public float damageMultiplier = 1.0f;
    public float speedMultiplier = 1.0f;
    public float healthMultiplier = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetMultipliers()
    {
        damageMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        healthMultiplier = 1.0f;
    }
}
