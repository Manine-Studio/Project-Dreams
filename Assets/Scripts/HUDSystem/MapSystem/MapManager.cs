using Misc;
using UnityEngine;

namespace HUDSystem.MapSystem
{
    public class MapManager : MonoBehaviour
    {
        private GameObject _xMapCanvas;
    
        // Start is called before the first frame update
        private void Start()
        {
            _xMapCanvas = transform.GetChild(0).gameObject;

            GameManager.Instance.XMapEventBus.Register(MapEventList.OPEN_MAP, OpenMap);
        }

        /// <summary>
        /// Toggle the map UI and trigger a refresh of all map-related conditions
        /// </summary>
        public void OpenMap(object[] param)
        {
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.CHECK_LOCATION);
            _xMapCanvas.SetActive(!_xMapCanvas.activeSelf);
        }
    }
}