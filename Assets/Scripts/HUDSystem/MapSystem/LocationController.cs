using System.Collections.Generic;
using Misc;
using Progress_System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUDSystem.MapSystem
{
    public class LocationController : ConditionChecker
    {
        [SerializeField] private string _sLocationSceneName;

        // Start is called before the first frame update
        private void Start()
        {
            _xImage = GetComponentInChildren<Image>();
            _xButton = GetComponentInChildren<Button>();
            
            _xButton.onClick.AddListener(ChangeScene);

            GameManager.Instance.XMapEventBus.Register(MapEventList.CHECK_LOCATION, CheckPreConditions);

            // Trigger an initial condition check when the object is created
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.CHECK_LOCATION);
        }

        /// <summary>
        /// Load the scene associated with this location button
        /// </summary>
        private void ChangeScene()
        {
            SceneManager.LoadScene(_sLocationSceneName);
        }
    }
}