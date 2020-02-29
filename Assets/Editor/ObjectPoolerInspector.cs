using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjectPooler pooler = (ObjectPooler) target;
        foreach (ObjectPooler.Pool pool in pooler.pools)
        {
            pool.tag = pool.prefab.name;
        }
    }
}
