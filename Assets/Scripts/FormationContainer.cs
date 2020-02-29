using UnityEngine;

public class FormationContainer : MonoBehaviour
{
    public void OnSelfDestruct()
    {
        foreach (Transform point in transform)
        {
            point.GetComponentInChildren<Enemy>().KillEnemy(true);
        }
    }
}
