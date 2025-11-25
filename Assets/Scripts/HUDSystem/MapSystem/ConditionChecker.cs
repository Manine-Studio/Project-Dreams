using Misc;
using Progress_System;
using UnityEngine;
using UnityEngine.UI;

namespace HUDSystem.MapSystem
{
    public class ConditionChecker : MonoBehaviour
    {
        [SerializeField] protected Condition _PreConditionsUnlock;
        [SerializeField] protected Condition _PreConditionsLock;
        [SerializeField] protected bool _bDefaultStatus;
        
        [SerializeField] protected Material _xEnableMaterial;
        [SerializeField] protected Material _xDisbleMaterial;
        
        protected Image _xImage;
        protected Button _xButton;
        
        // Use this for initialization
        private void Start()
        {
            _xButton = GetComponent<Button>();
            _xImage = GetComponent<Image>();

            GameManager.Instance.XInteractableEventBus.Register(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION, CheckPreConditions);
        }

        /// <summary>
        /// Check the configured conditions and enable/disable the UI element accordingly
        /// </summary>
        protected void CheckPreConditions(object[] param)
        {
            if (ConditionsUtils.CheckConditions(_PreConditionsUnlock))
                ChangeState(_xEnableMaterial, true);
            else if (ConditionsUtils.CheckConditions(_PreConditionsLock))
                ChangeState(_xDisbleMaterial, false);
            else
                // Apply default status if no condition is matched
                if (_bDefaultStatus)
                    ChangeState(_xEnableMaterial, true);
                else
                    ChangeState(_xDisbleMaterial, false);
        }

        /// <summary>
        /// Apply the graphic material and set the button's interactable state
        /// </summary>
        protected void ChangeState(Material mat, bool state)
        {
            _xImage.material = mat;
            _xButton.interactable = state;
        }
    }
}
