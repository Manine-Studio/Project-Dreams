using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;

/// <summary>
/// Handles triggering dialogues by using predefined dialogue assets and a dialogue elaborator.
/// </summary>
public class DialogueTrigger : DialogueTriggerBase
{
    [SerializeField] private DialogueSO _xDefaultDialogue; // Default dialogue if no specific dialogues are available
    public DialogueSO XDefaultDialogue { get => _xDefaultDialogue; set => _xDefaultDialogue = value; }
    
    /// <summary>
    /// Initiates the dialogue sequence by collecting all dialogues from the list and passing them 
    /// to the DialogueElaborator along with the default dialogue.
    /// </summary>
    public void StartDialogue()
    {
        // Create a list to store dialogues to be processed
        List<Dialogue> dialogueList = new List<Dialogue>();

        // Add dialogues from the serialized list to the local list
        for (int i = 0; i < _xDialogues.Count; i++)
        {
            dialogueList.Add(_xDialogues[i].Dialogue);
        }

        Dialogue defaultDialogue = null;

        if (dialogueList.Count == 0 && _xDefaultDialogue.Dialogue != null)
        {

            defaultDialogue = _xDefaultDialogue.Dialogue;
            dialogueList.Add(defaultDialogue);
        }

        // Pass the dialogues to the DialogueElaborator for processing
        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.START_DIALOGUE_ELAB, dialogueList, defaultDialogue);
    }
}