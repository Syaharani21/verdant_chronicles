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
    private bool isSkipping = false;    

    public bool IsDialogActive => isDialogActive; 

    void Start()
    {
        dialogPanel.SetActive(false); 
    }

    void Update()
    {
        if (isDialogActive && Input.GetKeyDown(KeyCode.E))
        {
            if (typingCoroutine != null)
            {
                // Jika efek mengetik sedang berlangsung, skip ke akhir teks
                isSkipping = true;
            }
            else
            {
                // Jika teks sudah selesai, lanjutkan ke baris berikutnya
                ShowLine();
            }
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

            // Mulai efek mengetik
            typingCoroutine = StartCoroutine(TypewriterEffect(dialogLines[currentLine].line));
        }
        else
        {
            EndDialog();
        }
    }

    private IEnumerator TypewriterEffect(string line)
    {
        dialogText.text = ""; 
        isSkipping = false;   // Reset status skip

        foreach (char c in line.ToCharArray())
        {
            if (isSkipping)
            {
                dialogText.text = line;
                break;
            }

            dialogText.text += c; 
            yield return new WaitForSeconds(typingSpeed); 
        }

        typingCoroutine = null;
        isSkipping = false;     
        currentLine++;          // Lanjutkan ke dialog berikutnya
    }

    public void EndDialog()
    {
        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
}
