using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogManager dialogManager; // Referensi ke dialog manager
    public GameObject dialogIcon;       // Ikon dialog

    private bool playerInRange = false;
    private bool hasTalked = false;

    private void Start()
    {
        if (dialogIcon != null)
        {
            dialogIcon.SetActive(true); // Tampilkan ikon di awal
        }
    }

    private void Update()
    {
        if (playerInRange && !hasTalked)
        {
            if (dialogIcon != null)
            {
                dialogIcon.SetActive(false); // Sembunyikan ikon dialog
            }

            if (dialogManager != null)
            {
                dialogManager.StartDialog(); // Mulai dialog
            }

            hasTalked = true; // Tandai dialog sudah dimulai
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (dialogIcon != null)
            {
                dialogIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (dialogIcon != null)
            {
                dialogIcon.SetActive(false);
            }
        }
    }
}
