using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSO))]
public class DialogueSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (DialogueSO)target;

        if(GUILayout.Button("Reset Parts", GUILayout.Height(40)))
        {
            script.ResetParts();
        }
        
    }
}
