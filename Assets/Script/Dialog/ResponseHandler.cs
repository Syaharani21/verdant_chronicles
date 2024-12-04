using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private dialogUi dialogueUi;

    private DialogObject dialogObject;
    
    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueUi = GetComponent<dialogUi>();
        responseButtonTemplate.gameObject.SetActive(false); // Pastikan template default tidak aktif
    }

    public void ShowResponses(Response[] responses)
    {
        if (responses == null || responses.Length == 0)
        {
            Debug.LogWarning("No responses available!");
            responseBox.gameObject.SetActive(false);
            return;
        }

        // Hapus tombol sebelumnya
        foreach (Transform child in responseContainer)
        {
            Destroy(child.gameObject);
        }
        tempResponseButtons.Clear();

        float responseBoxHeight = 0;

        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.SetActive(true); // Aktifkan tombol

            // Ubah teks tombol
            TMP_Text buttonText = responseButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = response.ResponseText;
            }
            else
            {
                Debug.LogWarning("TMP_Text component not found on response button!");
            }

            // Tambahkan event listener untuk respon
            Button button = responseButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnPickedResponse(response));
            }
            else
            {
                Debug.LogWarning("Button component not found on response button!");
            }

            tempResponseButtons.Add(responseButton);
            responseBoxHeight += responseButton.GetComponent<RectTransform>().sizeDelta.y;
        }

        // Perbarui ukuran response box
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true); // Tampilkan kotak respon
    }

    private void OnPickedResponse(Response response)
    {
        Debug.Log("Picked response: " + response.ResponseText);

        // Sembunyikan kotak respon
        responseBox.gameObject.SetActive(false);

        // Hapus tombol yang sudah dibuat
        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        // Jika ada dialog lanjutan, tampilkan
        if (response.DialogObject != null)
        {
            dialogueUi.ShowDialogue(response.DialogObject);
        }
        else
        {
            Debug.LogWarning("No follow-up dialogue for this response.");
        }
    }
}
