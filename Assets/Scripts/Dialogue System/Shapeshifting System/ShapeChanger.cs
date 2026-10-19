using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue_System.ShapeshiftingSystem
{
    [RequireComponent(typeof(Button))]
    public class ShapeChanger : MonoBehaviour
    {
        [SerializeField] private Shape _xShape;
        private Button _xButton;

        private void Start()
        {
            _xButton = GetComponent<Button>();
            _xButton.onClick.AddListener(ChangeShape);
        }

        private void ChangeShape()
        {
            ShapeshiftManager.XCurrentShape = _xShape;
        }
    }
}