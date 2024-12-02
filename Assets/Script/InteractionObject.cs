using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class InteractionObject : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string[] dialogLines; // Array untuk menyimpan dialog
    public float dialogDelay = 2f; // Waktu tunggu antara dialog

    private Canvas CanvasUI;
    private TextMeshProUGUI dialogText;
    private int currentLineIndex = 0;
    private bool isPlayerNear = false;

    private void Awake()
    {
        // Dapatkan referensi ke Canvas dan Text di dalamnya
        CanvasUI = GetComponentInChildren<Canvas>();
        if (dialogCanvas != null)
        {
            dialogText = CanvasUI.GetComponentInChildren<TextMeshProUGUI>();
            CanvasUI.enabled = false; // Sembunyikan canvas di awal
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!CanvasUI.enabled)
            {
                CanvasUI.enabled = true;
                StartCoroutine(DisplayDialog());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            CanvasUI.enabled = false; // Sembunyikan canvas saat player keluar
        }
    }

    private IEnumerator DisplayDialog()
    {
        while (currentLineIndex < dialogLines.Length)
        {
            dialogText.text = dialogLines[currentLineIndex];
            currentLineIndex++;
            yield return new WaitForSeconds(dialogDelay);
        }
    }
}
