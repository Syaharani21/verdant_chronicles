using UnityEngine;

public class DialogActivator : MonoBehaviour, InteractionObject
{
    [SerializeField] private DialogObject dialogObject;

    public void UpdateDialogObject(DialogObject dialogObject)
    {
        this.dialogObject = dialogObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out player player))
        {
            player.Interaction = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out player player))
        {
            if (player.Interaction is DialogActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interaction = null;
            }
        }
    }

    public void Interact(player player)
    {
        // Tambahkan event respons jika ada
        if (TryGetComponent(out DialogResponseEvent responseEvents))
        {
            player.DialogueUi.AddResponseEvents(responseEvents.Events);
        }
        
        // Tampilkan dialog
        player.DialogueUi.ShowDialogue(dialogObject);
    }
}
