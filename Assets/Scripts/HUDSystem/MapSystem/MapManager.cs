using Misc;
using UnityEngine;

namespace HUDSystem.MapSystem
{
    public class MapManager : MonoBehaviour
    {
        private GameObject _xMapCanvas;
    
        void Start()
        {
            _xMapCanvas = transform.GetChild(0).gameObject;
            GameManager.Instance.XMapEventBus.Register(MapEventList.OPEN_MAP, OpenMap);
        }

        public void OpenMap(object[] param)
        {
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.CHECK_LOCATION);
            _xMapCanvas.SetActive(!_xMapCanvas.activeSelf);
        }
    }
}
