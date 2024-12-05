using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    // Property to access dialogue
    public string[] Dialogue => dialogue;

    // Property to check if there are responses
    public bool HasResponses => Responses != null && Responses.Length > 0;

    // Property to access responses
    public Response[] Responses => responses;
}