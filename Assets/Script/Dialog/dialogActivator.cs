using UnityEngine;

public class DialogActivator : MonoBehaviour, InteractionObject // Capitalized class name
{
    [SerializeField] private DialogObject dialogObject;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out player player)) // Capitalized class name
        {
            player.Interaction = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out player player)) // Capitalized class name
        {
            if (player.Interaction is DialogActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interaction = null;
            }
        }
    }

    public void Interact(player player) // Capitalized class name
    {
        player.DialogueUi.ShowDialogue(dialogObject);
    }
}