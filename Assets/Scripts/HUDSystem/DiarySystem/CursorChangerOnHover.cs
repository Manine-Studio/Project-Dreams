using Misc;
using Progress_System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

delegate void FunctionParameter();

public class CursorChangerOnHover : MonoBehaviour
{
#if UNITY_EDITOR
    [Tooltip("point to a CursorType with the given Condition and Id")]
   public List<IdkThinkTomorrow> xConditionToCursorTypeList; // only here so it can be serialized
#endif

    Dictionary<KeyValuePair<Conditions, int>, CURSORTYPES> xConditionToCursorTypeMap = new();

    private EventTrigger _xEventTrigger;
    private Entry _xEntry = new();

    CURSORTYPES _xCurrentType = CURSORTYPES.none;

    private void Awake()
    {
        foreach(var conditionToCursorType in xConditionToCursorTypeList)
        {
            xConditionToCursorTypeMap.Add(new(conditionToCursorType.Condition, conditionToCursorType.ConditionId), conditionToCursorType.CursorType);
        }

        _xEventTrigger = GetComponent<EventTrigger>();

        // start listening for EventTrigger's PointerEnter
        AddFunctionToEventTrigger(EventTriggerType.PointerEnter, (eventData) => { ChangeCursor(); });

        // start listening for EventTrigger's PointerExit
        AddFunctionToEventTrigger(EventTriggerType.PointerExit, (eventData) => { ChangeCursorBackToNormal(); });

        GameManager.Instance.XInteractableEventBus.Register(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION, ChangeType);
        ChangeType();
    }

   
    private void ChangeType(params object[] obj)
    {
        ChangeType();
        ChangeCursor();
    }

    /// <summary>
    /// cheks if a condition the player has is inside the ConditionToCursorTypeMap and if so it stores the CursorType in currentType
    /// </summary>
    private void ChangeType()
    {

        Condition actualConditions = ConditionsUtils.GetConditions();

        foreach (KeyValuePair<Conditions, int> condition in actualConditions)
        {
            if (!xConditionToCursorTypeMap.TryGetValue(condition, out var type))
                continue;

            _xCurrentType = type;
            
            return;
        }
    }

    private void AddFunctionToEventTrigger(EventTriggerType triggerType, UnityAction<BaseEventData> function)
    {
        _xEntry = new();
        _xEntry.eventID = triggerType;
        _xEntry.callback.AddListener(function);
        _xEventTrigger.triggers.Add(_xEntry);
    }

    public void ChangeCursorBackToNormal()
    {
        GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, CURSORTYPES.none);
    }

    public void ChangeCursor()
    {
        GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, _xCurrentType);
    }
}

#if UNITY_EDITOR
[System.Serializable]
public struct IdkThinkTomorrow //TODO: think of a name 
{
    [Tooltip("Condition enum")]
    public Conditions Condition;
    [Tooltip("the id of the condition")]
    public int ConditionId;

    [Tooltip("Cursor type at the given condition and id \nex: knife 10 == CursorType = Important")]
    public CURSORTYPES CursorType;
}
#endif
