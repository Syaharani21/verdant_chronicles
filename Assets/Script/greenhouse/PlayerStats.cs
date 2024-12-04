using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float healthMultiplier = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of PlayerStats found. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}