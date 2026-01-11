using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;

/// <summary>
/// Handles triggering dialogues by using predefined dialogue assets and a dialogue elaborator.
/// </summary>
public class DialogueTriggerBase : MonoBehaviour
{
    // Serialized fields for Unity Inspector
    [SerializeField] protected List<DialogueSO> _xDialogues; // List of dialogue scriptable objects
    // Properties for external access
    public List<DialogueSO> XDialogues { get => _xDialogues; set => _xDialogues = value; }
    
    public void StartDialogue(DialogueSO dialogueSO)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();
        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.START_DIALOGUE_ELAB, dialogueList, dialogueSO.Dialogue);
    }
}