using Misc;

namespace Dialogue_System.ShapeshiftingSystem
{
    public class ShapeshiftManager
    {
        private static Shape _xCurrentShape;

        public static Shape XCurrentShape
        {
            get { return _xCurrentShape; }
            set
            {
                _xCurrentShape = value;
                GameManager.Instance.XShapeshiftEventBus.TriggerEvent(ShapeshiftEventList.ON_SHAPE_CHANGED);
            }
        }
    }

    [System.Serializable]
    public enum Shape
    {
        BAKONE,
        ELEANOR,
        GATTONE,
        GASTON,
        IKE,
        KERITH,
        LYNN,
        RHINA,
        ULDON,
    }
}