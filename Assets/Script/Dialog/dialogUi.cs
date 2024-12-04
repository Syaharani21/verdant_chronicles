using System.Collections;
using UnityEngine;
using TMPro;

public class dialogUi : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool IsOpen{get; private set;}

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

    private IEnumerator StepThroughDialogue(DialogObject dialogueObject)
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds before starting dialogue
        
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i]; 
            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if(i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)break;

            yield return null;
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

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                typewriterEffect.Stop();
            }
        }
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
      dialogueBox.SetActive(false);
      textLabel.text=string.Empty;
    }
}