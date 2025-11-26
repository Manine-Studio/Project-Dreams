using Misc;
using UnityEngine;

namespace HUDSystem.DiarySystem
{
    public class InteractableManager : MonoBehaviour
    {
        private void OnEnable()
        {
            GameManager.Instance.XInteractableEventBus.Register(InteractEventList.ON_CONDITION_CHANGE, RefreshInteractables);
        }

        private void OnDisable()
        {
            GameManager.Instance.XInteractableEventBus.Unregister(InteractEventList.ON_CONDITION_CHANGE, RefreshInteractables);
        }

        void RefreshInteractables(params object[] param) 
        {
            GameManager.Instance.XInteractableEventBus.TriggerEvent(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION);
        }
    }
}