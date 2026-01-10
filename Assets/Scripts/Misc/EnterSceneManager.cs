using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class EnterSceneManager : MonoBehaviour
{
    
    private DialogueTrigger _xDialogueTrigger;


    void Start()
    {
        _xDialogueTrigger = GetComponent<DialogueTrigger>();
        StartCoroutine("ACTIVATE_DIALOGUE_TRIGGER");
    }

    IEnumerator ACTIVATE_DIALOGUE_TRIGGER()
    {
        yield return new WaitForSeconds(0.5f);
        _xDialogueTrigger.StartDialogue();
    }
    

}
