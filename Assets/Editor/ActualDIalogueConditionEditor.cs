using Progress_System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ActualDialogueCondition))]
    public class ActualDialogueConditionEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            ActualDialogueCondition adc = (ActualDialogueCondition)target;
            adc.MConditions = ConditionsUtils.GetConditions();

            EditorUtility.SetDirty(adc);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Reset Conditions"))
            {
                ConditionsUtils.ResetConditionFile();
            }
        }
    }
}