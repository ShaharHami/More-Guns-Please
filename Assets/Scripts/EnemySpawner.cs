using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Formation[] formations;

    private void Start()
    {
//        float achmad = 43;
//        Vector3 achmadVector = new Vector3(achmad, achmad, achmad);
    }

    private void OnEnable()
    {
        EnemyDied.RegisterListener(ClearFormation);
    }

    private void OnDisable()
    {
        EnemyDied.UnregisterListener(ClearFormation);
    }

    public void SpawnFormation(Formation formation, string[] enemieTypes)
    {
        GameObject formationContainer = ObjectPooler.Instance.SpawnFromPool("FormationContainer", Vector3.zero, Quaternion.identity);
        formationContainer.transform.SetParent(transform);
        formationContainer.name = formation.formationName;
        formationContainer.transform.localPosition = Vector3.zero;
        foreach (var pos in formation.positions)
        {
            SpawnEnemy(enemieTypes[Random.Range(0, enemieTypes.Length)], pos + formation.initialPosition, Quaternion.identity, formationContainer.transform);
        }
        AnimateFormation(formationContainer);
    }

    private void AnimateFormation(GameObject formationContainer)
    {
        Animator animator = formationContainer.GetComponent<Animator>();
        animator.SetTrigger(formationContainer.name);
    }
    public void SpawnEnemy(string enemyType, Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject enemy = ObjectPooler.Instance.SpawnFromPool(enemyType, pos, rot);
        enemy.transform.SetParent(parent);
    }

    void ClearFormation(EnemyDied info)
    {
        int totalChildren = info.parent.childCount;
        int activeChildren = 0;
        for (int i = 0; i < totalChildren; i++)
        {
            if (info.parent.GetChild(i).gameObject.activeInHierarchy)
            {
                activeChildren++;
            }
        }

        if (activeChildren <= 0)
        {
            info.parent.gameObject.SetActive(false);
        }
        Debug.Log(activeChildren + " Enemies Left");
    }
}