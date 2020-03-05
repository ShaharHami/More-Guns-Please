using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool preWarmGradual;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static ObjectPooler Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            if (pool.preWarmGradual)
            {
                StartCoroutine(PreWarmGradual(pool));
            }
            else
            {
                PreWarm(pool);
            }
        }
    }

    private void PreWarm(Pool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(pool.tag, objectPool);
    }

    private IEnumerator PreWarmGradual(Pool pool)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
            yield return null;
        }

        poolDictionary.Add(pool.tag, objectPool);
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            print("no pool with the tag " + tag + " exists in the pool dictionary");
            return null;
        }
        GameObject obj;
        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            obj = poolDictionary[tag].Dequeue();
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.SetActive(true);
                poolDictionary[tag].Enqueue(obj);
                return obj;
            }
            poolDictionary[tag].Enqueue(obj);
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                obj = Instantiate(pool.prefab, pos, rot);
                poolDictionary[tag].Enqueue(obj);
                return obj;
            }
        }
        return null;
    }
}