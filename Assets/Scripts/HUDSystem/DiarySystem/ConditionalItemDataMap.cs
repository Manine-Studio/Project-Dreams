using UnityEngine;

namespace HUDSystem.DiarySystem
{
    [System.Serializable()]
    public class ConditionalItemDataMap : SerializableDictionaryBase<int, ItemModel> { }


    [System.Serializable]
    public struct ItemModel
    {
        public string sName;
        [TextArea]
        public string sDescription;
        public Sprite xObjectIcon;
        public Sprite xObjectImage;
    }

    public class DiaryUiItemData
    {
        public Conditions xCondition;
        public ItemModel xModel;
        public ObjectTypeEnum xType;

        public DiaryUiItemData(Conditions condition, ItemModel model, ObjectTypeEnum type)
        {
            this.xCondition = condition;
            this.xModel = model;
            this.xType = type;
        }
    }

    [System.Serializable]
    public class Data
    {
        public int index;
        public ItemModel itemModel;
    }
}