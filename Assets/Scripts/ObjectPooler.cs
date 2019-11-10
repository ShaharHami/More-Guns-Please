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
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            print("no pool with the tag " + tag + " exists in the pool dictionary");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rot;
        IpooledObject pooledObject = objectToSpawn.GetComponent<IpooledObject>();
        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}
