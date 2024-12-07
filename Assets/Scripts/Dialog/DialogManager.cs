using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogLine
    {
        public string speaker; 
        public string line;    
    }

    public DialogLine[] dialogLines; 
    public TextMeshProUGUI speakerText; 
    public TextMeshProUGUI dialogText;  
    public GameObject dialogPanel;      
    public GameObject nextButton;       
    private int currentLine = 0;       
    private bool isDialogActive = false;

    void Start()
    {
        dialogPanel.SetActive(false); 
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
            // Tampilkan nama pembicara dan dialog
            speakerText.text = dialogLines[currentLine].speaker;
            dialogText.text = dialogLines[currentLine].line;
            currentLine++;
        }
        else
        {
            EndDialog(); 
        }
    }

    public void EndDialog()
    {
        dialogPanel.SetActive(false); 
        isDialogActive = false;
    }
}
