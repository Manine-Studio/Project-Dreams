using System.Collections;
using System.Collections.Generic;
using Progress_System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogue_System.ShapeshiftingSystem
{
    public class ShapeshiftingDialogueSelector : MonoBehaviour
    {
        [Tooltip("used for knowing witch dialogue trigger to activate based on the shape of the player")] [SerializeField]
        private SerializableDictionaryBase<Shape, DialogueTrigger> _xShapeToDialogueTrigger;

        private SpriteRenderer _xSpriteRenderer;
        private EventTrigger _xEventTrigger;
        private EventTrigger.Entry _xEntry = new();

        void Start()
        {
            _xEventTrigger = GetComponent<EventTrigger>();

            _xEntry.eventID = EventTriggerType.PointerClick;
            _xEntry.callback.AddListener((eventData) => { SelectDialogueTrigger(); });
            _xEventTrigger.triggers.Add(_xEntry);
        }

        private void SelectDialogueTrigger()
        {
            foreach (var pair in _xShapeToDialogueTrigger)
            {
                if (pair.Key == ShapeshiftManager.XCurrentShape)
                {
                    pair.Value.StartDialogue();
                    return;
                }
            }
            
            // if it didn't find anything use the BakoneDefaultDialogue?
            _xShapeToDialogueTrigger[Shape.BAKONE].StartDialogue();
        }
    }
}