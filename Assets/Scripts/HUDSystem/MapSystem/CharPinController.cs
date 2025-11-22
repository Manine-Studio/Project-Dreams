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
        void Start()
        {
            _xImage = GetComponentInChildren<Image>();
            GameManager.Instance.XMapEventBus.Register(MapEventList.CHECK_LOCATION, CheckConditions);
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.CHECK_LOCATION);
        }

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
