using System.Collections.Generic;
using Misc;
using Progress_System;
using UnityEngine;
using UnityEngine.UI;

namespace HUDSystem.MapSystem
{
    public class CharPinController : MonoBehaviour
    {
        [SerializeField] private List<Condition> _xConditionCharList;
        [SerializeField] private List<Sprite> _xCharList;
        
        private Image _xImage;

        // Start is called before the first frame update
        private void Start()
        {
            _xImage = GetComponentInChildren<Image>();

            GameManager.Instance.XMapEventBus.Register(MapEventList.CHECK_LOCATION, CheckConditions);

            // Trigger an initial check when the object is created
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.CHECK_LOCATION);
        }

        /// <summary>
        /// Check all character conditions and update the displayed sprite accordingly
        /// </summary>
        private void CheckConditions(object[] param)
        {
            for (int i = 0; i < _xConditionCharList.Count; i++)
            {
                if (!ConditionsUtils.CheckConditions(_xConditionCharList[i])) continue;

                _xImage.sprite = _xCharList[i];
                break;
            }
        }
    }
}