using Misc;
using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    [SerializeField] 
    SerializableDictionaryBase<CURSORTYPES, Texture2D> CursorSprites;

    private void Awake()
    {
        GameManager.Instance.XInteractableEventBus.Register(InteractEventList.ON_CURSOR_CHANGED, ChangeMouseCursor);
    }

    private void OnDisable()
    {
        GameManager.Instance.XInteractableEventBus.Unregister(InteractEventList.ON_CURSOR_CHANGED, ChangeMouseCursor);
    }

    private void ChangeMouseCursor(object[] obj)
    {

        CURSORTYPES cursorType = (CURSORTYPES)obj[0];

        CursorSprites.TryGetValue(cursorType, out Texture2D texture);

        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}

public enum CURSORTYPES
{
    none,
    Unimportant,
    Important,
    Done
}