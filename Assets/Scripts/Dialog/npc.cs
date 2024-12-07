using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc : MonoBehaviour
{
    public DialogManager dialogManager; 
    public GameObject dialogIcon;       
    private bool hasTalked = false; 
   private void Start()
    {
       
        if (dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        } 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !hasTalked)
        {
            if (dialogIcon != null)
            {
                dialogIcon.SetActive(false); 
            }
            if (dialogManager != null)
            {
                dialogManager.StartDialog(); 
            }
            hasTalked = true; 
        }
    }
}
