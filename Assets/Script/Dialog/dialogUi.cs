using System.Collections;
using UnityEngine;
using TMPro;

public class dialogUi : MonoBehaviour // Capitalized class name
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;


    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();       
        CloseDialogueBox();
    }

    public void ShowDialogue(DialogObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponeEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogObject dialogueObject)
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds before starting dialogue
        
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            
            yield return StartCoroutine(typewriterEffect.Run(dialogue, textLabel));

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space)); // Wait for space key to proceed
            yield return new WaitForSeconds(1);
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    public IEnumerator Run(string text, TMP_Text textLabel)
    {
        textLabel.text = string.Empty; // Clear text label before starting typing

        foreach (char letter in text)
        {
            textLabel.text += letter; // Append one letter at a time
            yield return new WaitForSeconds(0.05f); // Typing speed (adjustable)
        }
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }
}