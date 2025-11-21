using Misc;
using UnityEngine;

namespace HUDSystem
{
    public class HUDManager : MonoBehaviour
    {
        public void OpenMap()
        {
            GameManager.Instance.XMapEventBus.TriggerEvent(MapEventList.OPEN_MAP);
        }
    }
}
