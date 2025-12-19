using Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

delegate void FunctionParameter();

public class CursorChangerOnHover : MonoBehaviour
{
    [SerializeField, Tooltip("use the name of the object/charachter_SceneName \nEx: eleanor_TestScene")]
    string isDoneKey;
    [SerializeField] bool IsImportant;
    [SerializeField] bool isCharachter = false;
    bool isDone = false;


    private EventTrigger _xEventTrigger;
    private Entry _xEntry = new();

    private void Awake()
    {
        _xEventTrigger = GetComponent<EventTrigger>();

        // start listening for EventTrigger's PointerEnter
        AddFunctionToEventTrigger(EventTriggerType.PointerEnter, (eventData) => { ChangeCursor(); });

        // start listening for EventTrigger's PointerExit
        AddFunctionToEventTrigger(EventTriggerType.PointerExit, (eventData) => { ChangeCursorBackToNormal(); });

        // start listening for EventTrigger's PointerClick
        AddFunctionToEventTrigger(EventTriggerType.PointerClick, (eventData) => { ChangeCursorToIsDone(); });

        if (PlayerPrefs.GetInt(isDoneKey) == 0)
            isDone = false;
        else
            isDone = true;
        
    }

    private void OnDisable()
    {
        if (isDone)
            PlayerPrefs.SetInt(isDoneKey, 1);
        else
            PlayerPrefs.SetInt(isDoneKey, 0);
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

    private void ChangeCursor()
    {
        if (isDone)
            GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, CURSORTYPES.Done);
        else if (isCharachter)
            GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, CURSORTYPES.Character);
        else if (IsImportant)
            GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, CURSORTYPES.Important);
        else
            GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.ON_CURSOR_CHANGED, CURSORTYPES.Unimportant);
    }

    private void ChangeCursorToIsDone(params object[] param)
    {
        if (isDone)
            return;

        isDone = true;
        ChangeCursor();
        // register 
    }
}


