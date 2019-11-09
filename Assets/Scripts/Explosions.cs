using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour
{
    public Explosion[] explosions;
    static private Dictionary<string, Explosion> explosionsDictionary;
    void Start()
    {
        explosionsDictionary = new Dictionary<string, Explosion>();
        foreach (Explosion explosion in explosions)
        {
            explosionsDictionary.Add(explosion.name, explosion);
            // explosion.explosionVisualEffect.SetActive(false);
        }
    }
    void Update()
    {
        
    }
    public void Explode(string name, Vector3 instantiationPosition, float delayBeforeDestroy)
    {
        if (explosionsDictionary.ContainsKey(name))
        {
            GameObject explosionGO = explosionsDictionary[name].explosionVisualEffect;
            GameObject explosionInstance = Instantiate(explosionGO, instantiationPosition, Quaternion.identity);
            explosionInstance.SetActive(true);
            Destroy(explosionInstance, delayBeforeDestroy);
        }
        else
        {
            print ("Did you misspell your explosion?!");
        }
    }
}
[System.Serializable]
public class Explosion
{
    public string name;
    public GameObject explosionVisualEffect;
}
