using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogLine
    {
        public string speaker; 
        public string line;    
        public Sprite sprite;  
    }

    public DialogLine[] dialogLines; 
    public TextMeshProUGUI speakerText; 
    public TextMeshProUGUI dialogText;  
    public Image speakerImage;          
    public GameObject dialogPanel;      
    public float typingSpeed = 0.05f;    

    private int currentLine = 0;        
    private bool isDialogActive = false;
    private Coroutine typingCoroutine;  

    void Start()
    {
        dialogPanel.SetActive(false);
    }

    void Update()
    {
        // Lanjutkan dialog dengan tombol E
        if (isDialogActive && Input.GetKeyDown(KeyCode.E) && typingCoroutine == null)
        {
            ShowLine(); 
        }
    }

    public void StartDialog()
    {
        if (!isDialogActive)
        {
            dialogPanel.SetActive(true);
            currentLine = 0;
            isDialogActive = true;
            ShowLine();
        }
    }

    public void ShowLine()
    {
        if (currentLine < dialogLines.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Tampilkan gambar dan nama pembicara
            speakerText.text = dialogLines[currentLine].speaker;
            speakerImage.sprite = dialogLines[currentLine].sprite;

            // Mulai efek mengetik untuk teks
            typingCoroutine = StartCoroutine(TypewriterEffect(dialogLines[currentLine].line));
            currentLine++;
        }
        else
        {
            EndDialog(); 
        }
    }

    private IEnumerator TypewriterEffect(string line)
    {
        dialogText.text = ""; // Kosongkan dialog sebelum mulai mengetik
        foreach (char c in line.ToCharArray())
        {
            dialogText.text += c; // Tambahkan karakter satu per satu
            yield return new WaitForSeconds(typingSpeed); // Tunggu sebelum menambahkan karakter berikutnya
        }

        typingCoroutine = null; // Ketikan selesai
    }

    public void EndDialog()
    {
        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
