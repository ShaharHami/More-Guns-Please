using System.Collections;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Formation[] formations;

    private void OnEnable()
    {
        EnemyDied.RegisterListener(ClearFormation);
        ReachedPoint.RegisterListener(ParentEnemy1);
    }

    private void OnDisable()
    {
        EnemyDied.UnregisterListener(ClearFormation);
        ReachedPoint.UnregisterListener(ParentEnemy1);
    }

    public void SpawnFormation(Formation formation, string[] enemieTypes)
    {
        GameObject topContainer = ObjectPooler.Instance.SpawnFromPool("TopContainer", formation.initialPosition, Quaternion.identity);
        topContainer.transform.SetParent(null);
        GameObject formationContainer = ObjectPooler.Instance.SpawnFromPool("FormationContainer", Vector3.zero, Quaternion.identity);
        formationContainer.transform.SetParent(null);
        topContainer.transform.SetParent(transform);
        formationContainer.transform.SetParent(topContainer.transform);
        formationContainer.name = formation.formationName;
        foreach (var pos in formation.positions)
        {
            GameObject positionMarker = ObjectPooler.Instance.SpawnFromPool("PositionMarker", pos, Quaternion.identity);
            positionMarker.name = pos.ToString();
            positionMarker.transform.SetParent(formationContainer.transform);
            positionMarker.transform.localPosition = pos;
        }
        StartCoroutine(PopulateFormation(formation, enemieTypes, formationContainer));
        AnimateFormation(formationContainer);
    }

    IEnumerator PopulateFormation(Formation formation, string[] enemieTypes, GameObject formationContainer)
    {
        for (int i = 0; i < formation.positions.Length; i++)
        {
            SpawnEnemy(enemieTypes[Random.Range(0, enemieTypes.Length)], 
                formation.positions[i], 
                Quaternion.identity,
                formationContainer.transform, 
                i);
            yield return new WaitForSeconds(1f);
        }
        foreach (var pos in formation.positions)
        {
        }
    }
    private void AnimateFormation(GameObject formationContainer)
    {
        Animator animator = formationContainer.GetComponent<Animator>();
        animator.SetTrigger(formationContainer.name);
    }
    public void SpawnEnemy(string enemyType, Vector3 pos, Quaternion rot, Transform parent, int parentIndex)
    {
        if (parent.childCount <= parentIndex) return;
        Enemy enemy = ObjectPooler.Instance.SpawnFromPool(enemyType, Vector3.zero, rot).GetComponent<Enemy>();
        enemy.transform.SetParent(null);
        enemy.transform.position = new Vector3(0,0,150f);
        Transform parentTransform = parent.GetChild(parentIndex);
        enemy.FlyToPoint(parentTransform);
    }

    void ParentEnemy1(ReachedPoint info)
    {
        info.objTransform.SetParent(info.parentTransform);
    }
    void ClearFormation(EnemyDied info)
    {
        if (info.parent == null) return;
        int activeChildren = 0;
        foreach (Transform child in info.parent)
        {
            if (child.gameObject.activeSelf && child.childCount > 0)
            {
                activeChildren++;
            }            
        }
        if (activeChildren <= 0)
        {
            info.parent.gameObject.SetActive(false);
            info.parent.parent.gameObject.SetActive(false);
            info.parent.parent.transform.SetParent(null);
            info.parent.transform.SetParent(null);
        }
    }
}