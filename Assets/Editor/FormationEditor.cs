using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Formation))]
public class FormationEditor : Editor
{
    UnityEditor.Animations.AnimatorController controller;
    private List<string> triggers;
    private List<string> enemyTypes;
    public bool[] showEnemy;
    public bool enemiesEnabled;
    private Formation formation;

    private void OnEnable()
    {
        if (triggers == null)
        {
            triggers = new List<string>();
        }

        formation = (Formation) target;
        GetParams();
        enemyTypes = new List<string>();
        GetPrefabs<Enemy>("Prefabs/");
        showEnemy = new bool[enemyTypes.Count];
        if (formation.EnemyTypes.Count > 0)
        {
            foreach (string enemyType in formation.EnemyTypes)
            {
                for (int j = 0; j < showEnemy.Length; j++)
                {
                    if (enemyType == enemyTypes[j])
                    {
                        showEnemy[j] = true;
                    }
                }
            }
        }
        else
        {
            showEnemy[0] = true;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            showEnemy[i] = EditorGUILayout.Toggle(enemyTypes[i], showEnemy[i]);
        }

        for (int i = 0; i < showEnemy.Length; i++)
        {
            if (showEnemy[i] && !formation.EnemyTypes.Contains(enemyTypes[i]))
            {
                formation.EnemyTypes.Add(enemyTypes[i]);
            }

            if (!showEnemy[i] && formation.EnemyTypes.Contains(enemyTypes[i]))
            {
                formation.EnemyTypes.Remove(enemyTypes[i]);
            }
        }

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
        if (runtimeController == null)
        {
            Debug.LogErrorFormat("RuntimeAnimatorController must not be null.");
            return;
        }

        var controller =
            AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(
                AssetDatabase.GetAssetPath(runtimeController));
        if (controller == null)
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

    public List<T> GetPrefabs<T>(string path)
    {
        List<T> prefabs = new List<T>();
        Object[] objects = Resources.LoadAll(path);
        foreach (var obj in objects)
        {
            if (obj is GameObject)
            {
                GameObject go = (GameObject) obj;
                var t = go.GetComponent<T>();
                if (t != null)
                {
                    prefabs.Add(t);
                    enemyTypes.Add(go.name);
                }
            }
        }

        return prefabs;
    }

//    public List<string> GetPrefabNames<T>(List<T> objects)
//    {
//        List<string> names = new List<string>();
//        foreach (var obj in objects)
//        {
//            var e = (GameObject) obj;
//            names.Add(obj.ToString());
//        }
//
//        return names;
//    }
}