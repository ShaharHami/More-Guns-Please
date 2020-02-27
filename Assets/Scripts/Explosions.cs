using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Explosions : MonoBehaviour
{
    public Explosion[] explosions;
    static private Dictionary<string, Explosion> explosionsDictionary;
    void Start()
    {
        explosionsDictionary = new Dictionary<string, Explosion>();
        foreach (Explosion explosion in explosions)
        {
            explosionsDictionary.Add(explosion.explosionName, explosion);
        }
    }
    public void Explode(string explosionName, Vector3 instantiationPosition, float delayBeforeDestroy)
    {
        if (explosionsDictionary.ContainsKey(explosionName))
        {
            GameObject explosionInstance = ObjectPooler.Instance.SpawnFromPool(explosionName, instantiationPosition, Quaternion.identity);
            StartCoroutine(TimedDisable(explosionInstance, delayBeforeDestroy));
        }
        else
        {
            print ("Did you misspell your explosion?!");
        }
    }

    IEnumerator TimedDisable(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
[System.Serializable]
public class Explosion
{
    public string explosionName;
    public GameObject explosionVisualEffect;
}
