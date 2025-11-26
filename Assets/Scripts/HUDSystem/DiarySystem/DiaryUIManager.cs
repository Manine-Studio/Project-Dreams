using System.Collections.Generic;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUDSystem.DiarySystem
{
    public class DiaryUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject DiaryUiPannel;

        [SerializeField] private Image xItemPortrait;
        [SerializeField] private TextMeshProUGUI xDescription;
        [SerializeField] private Sprite xItemSlotDefaultSprite;

        [SerializeField] private ItemSlot[] xItemSlots = new ItemSlot[16];

        private List<DiaryUiItemData> _xItemsList = new();
        private List<DiaryUiItemData> _xCharactersList = new();
        private List<DiaryUiItemData> _xLocationsList = new();

        private int _iCurrentPageIndex = 0;

        //private bool _bHasLoaded;

        private ObjectTypeEnum _xCurrentUIType = ObjectTypeEnum.Item;


        private void OnEnable()
        {
            GameManager.Instance.XInteractableEventBus.Register(InteractEventList.ON_DIARY_CHANGE, UpdateDiary);
            GameManager.Instance.XInteractableEventBus.Register(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);
        }


        private void OnDisable()
        {
            GameManager.Instance.XInteractableEventBus.Unregister(InteractEventList.ON_DIARY_CHANGE, UpdateDiary);
            GameManager.Instance.XInteractableEventBus.Unregister(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);
        }

        public void OpenUI()
        {
            //if (_bHasLoaded)
            //{
            //    DiaryUiPannel.SetActive(true);
            //    return;
            //}

            switch ((int)_xCurrentUIType)
            {
                case 1:
                    LoadUI(ref _xItemsList);
                    break;

                case 2:
                    LoadUI(ref _xCharactersList);
                    break;

                case 3:
                    LoadUI(ref _xLocationsList);
                    break;

                default:
                    Debug.LogError($"how did this even happen? error in openUI no OBJECT_TYPE");
                    break;
            }

            DiaryUiPannel.SetActive(true);
        }

        public void CloseUI()
        {
            DiaryUiPannel.SetActive(false);
        }

        private void UpdateDiary(params object[] param)
        {
            List<DiaryUiItemData> paramMap = (List<DiaryUiItemData>)param[0];

            foreach (var paramItem in paramMap)
            {
                switch ((int)paramItem.xType)
                {
                    case 1:
                        CheckDuplicateInsideList(paramItem, ref _xItemsList);
                        //LoadUI(ref _xItemsList);
                        break;

                    case 2:
                        CheckDuplicateInsideList(paramItem, ref _xCharactersList);
                        //LoadUI(ref _xCharactersList);
                        break;

                    case 3:
                        CheckDuplicateInsideList(paramItem, ref _xLocationsList);
                        //LoadUI(ref _xLocationsList);
                        break;

                    default:
                        Debug.LogError($"the item {paramItem.xModel.sName} does not have a type");
                        break;
                }
            }

            void CheckDuplicateInsideList(DiaryUiItemData paramItem, ref List<DiaryUiItemData> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (paramItem.xCondition != list[i].xCondition)
                        continue;
                    if (paramItem.xType != list[i].xType)
                        continue;

                    list[i] = paramItem;
                    return;
                }
                list.Add(paramItem);
                return;
            }
        }

        private void LoadUI(ref List<DiaryUiItemData> list)
        {
            // basicly the first index in the list that apears in the slots 
            // ex: slots.lengh = 2 list.count = 6, if pageIndex = 2 then we need the item in the list[4] slot 
            var firstIndexOnPage = xItemSlots.Length * _iCurrentPageIndex; 

            #region |loop the pages|
            if (firstIndexOnPage >= list.Count)
            {
                _iCurrentPageIndex = 0;
                firstIndexOnPage = xItemSlots.Length * _iCurrentPageIndex; 
            }

            else if (firstIndexOnPage < 0)
            {
                _iCurrentPageIndex = list.Count / xItemSlots.Length;
                firstIndexOnPage = xItemSlots.Length * _iCurrentPageIndex;
            }
            
            #endregion |loop the pages|


            int itemSlotIndex = 0;

            for (int i = firstIndexOnPage; i < list.Count; i++)
            {
                if (i >= list.Count || itemSlotIndex >= xItemSlots.Length)
                    break;

                ItemModel listItem = list[i].xModel;

                // change the icons of the items
                xItemSlots[itemSlotIndex].xIcon.sprite = listItem.xObjectIcon;
                xItemSlots[itemSlotIndex].xName.text = listItem.sName;
                xItemSlots[itemSlotIndex].xItemImage = listItem.xObjectImage;
                xItemSlots[itemSlotIndex].sDescription = listItem.sDescription;

                itemSlotIndex++;
            }

            if (itemSlotIndex >= xItemSlots.Length)
                return;

            for (int i = itemSlotIndex; i < xItemSlots.Length; i++)
            {
                xItemSlots[i].xIcon.sprite = xItemSlotDefaultSprite;
                xItemSlots[i].xName.text = "";
                xItemSlots[i].xItemImage = null;
                xItemSlots[i].sDescription = "";
            }
        }

        /// <summary>
        /// changes the item loaded
        /// </summary>
        /// <param name="pageTurnDirection">-1 = previous page | 1 = next page </param>
        public void ChangePage(int pageTurnDirection)
        {
            _iCurrentPageIndex += pageTurnDirection;

            
            LoadUI(ref _xItemsList);
        }

        public void ChangeType(ObjectTypeEnum type)
        {
            _xCurrentUIType = type;
            _iCurrentPageIndex = 0;
            LoadUI(ref _xItemsList);
        }

        private void OnItemSlotPressed(object[] obj)
        {
            ItemSlot itemSlot = (ItemSlot)obj[0];

            xItemPortrait.sprite = itemSlot.xItemImage;
            xDescription.text = itemSlot.sDescription;
        }


    }
}