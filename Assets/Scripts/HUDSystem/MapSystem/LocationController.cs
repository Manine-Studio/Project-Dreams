using Misc;
using Progress_System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUDSystem.MapSystem
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField] private string _sLocationSceneName;
        
        [SerializeField] private bool _bDefaultStatus;
        
        [SerializeField] private Material _xEnableMaterial;
        [SerializeField] private Material _xDisbleMaterial;
        
        //Conditions needed to show this dialogue
        [SerializeField] private Condition _PreConditionsUnlock;
        [SerializeField] private Condition _PreConditionsLock;
        
        private Image _xImage;
        private Button _xButton;
        
        void Start()
        {
            _xImage = GetComponentInChildren<Image>();
            _xButton = GetComponentInChildren<Button>();
            
            _xButton.onClick.AddListener(ChangeScene);
            GameManager.Instance.XMapEventBus.Register(MapEventList.CHECK_LOCATION, CheckConditions);
        }

        private void CheckConditions(object[] param)
        {
            if (ConditionsUtils.CheckConditions(_PreConditionsUnlock))
                ChangeState(_xEnableMaterial, true);
            else if (ConditionsUtils.CheckConditions(_PreConditionsLock))
                ChangeState(_xDisbleMaterial, false);
            else
                if (_bDefaultStatus)
                    ChangeState(_xEnableMaterial, true);
                else
                    ChangeState(_xDisbleMaterial, false);
        }
        
        private void ChangeState(Material mat, bool state)
        {
            _xImage.material = mat;
            _xButton.interactable = state;
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(_sLocationSceneName);
        }
    }
}
