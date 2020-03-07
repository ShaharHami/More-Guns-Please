using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Explosions))]
public class ExplosionManagerEditor : Editor
{
    public List<string> explosions;

    private void OnEnable()
    {
        explosions = new List<string>();
        Explosions explosionManagerGo = (Explosions) target;
        GetPrefabs<IExplode>("Prefabs/");
        explosionManagerGo.explosions = explosions;
//        if (ObjectPooler.Instance != null)
//        {
//            foreach (var pool in ObjectPooler.Instance.pools.Where(
//                pool => pool.prefab.GetComponent<IExplode>() != null))
//            {
//                explosions.Add(pool.tag);
//                explosionManagerGo.explosions = explosions;
//            }
//        }
    }
    public List<GameObject> GetPrefabs<T>(string path)
    {
        List<GameObject> prefabs = new List<GameObject>();
        Object[] objects = Resources.LoadAll(path);
        foreach (var obj in objects)
        {
            if (obj is GameObject)
            {
                GameObject go = (GameObject) obj;
                var t = go.GetComponent<T>();
                var tInChildren = go.GetComponentInChildren<T>();
                if (t != null)
                {
                    prefabs.Add(go);
                    explosions.Add(go.name);
                }
                else if (tInChildren != null)
                {
                    prefabs.Add(go);
                    explosions.Add(go.name);
                }
            }
        }

        return prefabs;
    }
}