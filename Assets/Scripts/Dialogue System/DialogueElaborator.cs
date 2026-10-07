using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Misc;
using Progress_System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueElaborator : MonoBehaviour
{
    private bool _bIsRunning = false;

    private Dialogue _Dialogue;

    private List<Sentence> _xSentences;

    private int _CurrentMonologueIndex;
    private int _CurrentSentenceIndex;


    // Use this for initialization
    private void Start()
    {
        _xSentences = new();

        GameManager.Instance.XDialogueEventBus.Register(DialogueEventList.START_DIALOGUE_ELAB, StartDialogue);
    }

    /// <summary>
    /// Method used for starting a specified dialogue, need a list of dialogues with preconditions and a default dialogue
    /// </summary>
    /// <param name="dialogueList"></param>
    /// <param name="defaultDialogue"></param>
    public void StartDialogue(object[] param)
    {
        List<Dialogue> dialogueList = (List<Dialogue>)param[0];
        Dialogue defaultDialogue = null;
        if (param.Length > 1)
        {
            defaultDialogue = (Dialogue)param[1];
        }

        if (!_bIsRunning)
        {
            if (dialogueList != null && dialogueList.Count > 0)
            {
                //checking which dialogue from the list is the one who respects the preconditions
                for (int i = 0; i < dialogueList.Count; i++)
                {
                    if (ConditionsUtils.CheckConditions(dialogueList[i].PreConditions))
                    {
                        _Dialogue = dialogueList[i];
                        break;
                    }
                }
            }

            if (_Dialogue == null)
            {
                _Dialogue = defaultDialogue;
            }

            if (_Dialogue != null)
            {
                GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.START_DIALOGUE);

                if (_Dialogue.DialogueMusicBackground != null)
                {
                    DialogueSoundManager.PlayOnLoop(_Dialogue.DialogueMusicBackground, _Dialogue.StartingLoopPoint);
                }

                _bIsRunning = true;
                StartMonologue();
            }
        }
    }

    /// <summary>
    /// Start the monologue based on CurrentMonologueIndex and show the first sentence
    /// </summary>
    public void StartMonologue()
    {
        if (_Dialogue != null)
        {
            GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.CHANGE_NAME, _Dialogue.DialogueParts[_CurrentMonologueIndex].SName);
            //ClearCurrent();
            // _xSentences.Clear();
            _xSentences = _Dialogue.DialogueParts[_CurrentMonologueIndex].Sentences;
            

            DisplayNextSentence(); // in this case it's the first sentence
        }
    }

    /// <summary>
    /// Show the first sentence available in the list of CurrentText
    /// </summary>
    public void DisplayNextSentence()
    {
        if (TypingEffect.Instance != null && !TypingEffect.Instance.IsCurrentSentenceFinished)
        {
            TypingEffect.Instance.TypeFully();
            return;
        }

        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.RESET_CHARACTER); // resets the transform of the characters

        if (_Dialogue == null || _Dialogue.DialogueParts == null || _Dialogue.DialogueParts.Count == 0)
            return;

        // if (_sCurrentText != null && _Dialogue != null && _Dialogue.DialogueParts != null && _Dialogue.DialogueParts.Count > 0) do the rest of the code

        //If i reached the last sentence 
        if (_CurrentSentenceIndex == _xSentences.Count)
        {
            // if the current monologue wasn't the last, start the next monologue
            if (_CurrentMonologueIndex < _Dialogue.DialogueParts.Count - 1)
            {
                _CurrentSentenceIndex = 0;
                _CurrentMonologueIndex++;
                StartMonologue();
            }
            else // if it was the last
            {
                if (_Dialogue.HasChoices)
                {
                    List<List<Choice>> listDialogues = new List<List<Choice>>();
                    listDialogues.Add(_Dialogue.DialogueChoices);
                    EndDialogue();
                    GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.START_CHOICE, listDialogues);
                }
                else
                {
                    EndDialogue();
                    GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.CHECK_POST_INTERACTION);
                }
            }

            return;
        }

        TypeSentence();
        _CurrentSentenceIndex++;
    }

    /// <summary>
    /// Type letter by letter the entire senteces passed by param in UI
    /// </summary>
    /// <param name="sentence"></param>
    /// <returns></returns>
    private void TypeSentence()
    {
        string sentence = _xSentences[_CurrentSentenceIndex].SSentence;
        
        // Sprite[] images = _xSentences[_CurrentSentenceIndex]._XSpriteAnimMap.XSprites;
        // AnimationClip[] animations = _xSentences[_CurrentSentenceIndex]._XSpriteAnimMap.XAnimations;

        // List<Sprite[]> list = new List<Sprite[]>();
        // list.Add(images);

        //GetFromCurrent(out sentence, out image, out animations);

        if (_xSentences[_CurrentSentenceIndex].SFXAudio != null)
            DialogueSoundManager.PlayOneShotSound(_xSentences[_CurrentSentenceIndex].SFXAudio);


        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.CHANGE_SENTENCE, "");
        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.CHANGE_SENTENCE, sentence);
        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.CHANGE_IMAGE, _xSentences[_CurrentSentenceIndex]._XSpriteAnimMap);
    }

    /// <summary>
    /// Stop dialogue, reset all to default values and hide text box in UI
    /// </summary>
    private void EndDialogue()
    {
        ConditionsUtils.ApplyCondition(_Dialogue.PostConditions);

        _CurrentMonologueIndex = 0;
        _CurrentSentenceIndex = 0;

        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.END_DIALOGUE, _Dialogue);
        GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION);

        GameManager.Instance.XDialogueEventBus.TriggerEvent(DialogueEventList.END_DIALOGUE);

        _bIsRunning = false;
        if (_Dialogue != null && _Dialogue.HasSceneChange && !string.IsNullOrEmpty(_Dialogue.TargetSceneName))
        {
            SceneManager.LoadScene(_Dialogue.TargetSceneName);
        }

        _Dialogue = null;
    }
}