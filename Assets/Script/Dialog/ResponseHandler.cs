using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private dialogUi dialogueUi; // Corrected class name

    private DialogObject dialogObject;
    private ResponeEvent[] responseEvents;
    
    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueUi = GetComponent<dialogUi>(); // Corrected class name
        responseButtonTemplate.gameObject.SetActive(false); // Ensure the template is not active
    }

    public void AddResponseEvents(ResponeEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponses(Response[] responses)
    {
        if (responses == null || responses.Length == 0)
        {
            Debug.LogWarning("No responses available!");
            responseBox.gameObject.SetActive(false);
            return;
        }

        // Clear previous buttons
        foreach (Transform child in responseContainer)
        {
            Destroy(child.gameObject);
        }
        tempResponseButtons.Clear();

        float responseBoxHeight = 0;

        for (int i = 0; i < responses.Length; i++) // Use a for loop to get the index
        {
            Response response = responses[i]; // Corrected variable shadowing
            int responseIndex = i;
            
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.SetActive(true); // Activate the button

            // Change button text
            TMP_Text buttonText = responseButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = response.ResponseText;
            }
            else
            {
                Debug.LogWarning("TMP_Text component not found on response button!");
            }

            // Add event listener for response
            Button button = responseButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnPickedResponse(response, responseIndex));
            }
            else
            {
                Debug.LogWarning("Button component not found on response button!");
            }

            tempResponseButtons.Add(responseButton);
            responseBoxHeight += responseButton.GetComponent<RectTransform>().sizeDelta.y; // Add button height
        }

        // Update response box size
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true); // Show response box
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        Debug.Log("Picked response: " + response.ResponseText);

        // Hide response box
        responseBox.gameObject.SetActive(false);

        // Destroy created buttons
        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex < responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        dialogueUi.ShowDialogue(response.DialogObject);
    }
}