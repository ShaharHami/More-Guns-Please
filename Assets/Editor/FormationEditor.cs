using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Formation))]
public class FormationEditor : Editor
{
    UnityEditor.Animations.AnimatorController controller;
    private List<string> triggers;
    public int index;
    private Formation formation;
    
    private void OnEnable()
    {
        if (triggers == null)
        {
            triggers = new List<string>();
        }
        formation = (Formation)target;
        GetParams();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (formation.animator == null)
        {
            EditorGUILayout.HelpBox($"No Animator Attached.\nPlease Attach Animator.", MessageType.Warning);
            return;
        }
        if (triggers.Count == 0)
        {
            GetParams();
            if (triggers.Count == 0)
            {
                EditorGUILayout.HelpBox($"No Triggers Found On Animator.", MessageType.Warning);
                return;
            }
        }
        GUIContent triggersLabel = new GUIContent("Animation Trigger");
        formation.triggerIdx = EditorGUILayout.Popup(triggersLabel, formation.triggerIdx, triggers.ToArray());
        formation.triggerName = triggers[formation.triggerIdx];
    }
    public void GetParams()
    {
        if (formation.animator == null) return;
        var runtimeController = formation.animator.runtimeAnimatorController;
        if(runtimeController == null)
        {
            Debug.LogErrorFormat("RuntimeAnimatorController must not be null.");
            return;
        }
        var controller = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(runtimeController));
        if(controller == null)
        {
            Debug.LogErrorFormat("AnimatorController must not be null.");
            return;
        }
        foreach (var parameter in controller.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                triggers.Add(parameter.name);
            }
        }
    }
}