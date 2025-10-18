using System.Collections;
using System.Collections.Generic;
using Misc;
using Progress_System;
using UnityEngine;

[RequireComponent(typeof(DialogueTriggerBase))]
public class PostInteractionTrigger : MonoBehaviour
{
    private DialogueTriggerBase _xDialogueTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        _xDialogueTrigger  = GetComponent<DialogueTriggerBase>();
        
        GameManager.Instance.XDialogueEventBus.Register(DialogueEventList.CHECK_POST_INTERACTION, CheckPostInteraction);
    }

    private void CheckPostInteraction(object[] param)
    {
        DialogueSO dialogue = null;
        //checking which dialogue from the list is the one who respects the preconditions
        for(int i = 0; i < _xDialogueTrigger.XDialogues.Count; i++)
        {
            if (ConditionsUtils.CheckConditions(_xDialogueTrigger.XDialogues[i].Dialogue.PreConditions))
            {
                dialogue = _xDialogueTrigger.XDialogues[i];
                break;
            }
        }

        if (dialogue == null) return;
        _xDialogueTrigger.StartDialogue(dialogue);
    }
}
