using EventCallbacks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Formation[] formations;

    private void OnEnable()
    {
        FormationDead.RegisterListener(KillFormation);
    }

    private void OnDisable()
    {
        FormationDead.UnregisterListener(KillFormation);
    }

    public void SpawnFormation(Formation formation, string[] enemieTypes)
    {
        Vector3 initialPosition = new Vector3(
            Random.Range(formation.initialPositionX.x, formation.initialPositionX.y),
            0,
            Random.Range(formation.initialPositionZ.x, formation.initialPositionZ.y)
        );
        GameObject topContainer = ObjectPooler.Instance.SpawnFromPool("TopContainer", initialPosition, Quaternion.identity);
        SingleFormationManager formationManager = topContainer.GetComponent<SingleFormationManager>();
        formationManager.formation = formation;
        topContainer.transform.SetParent(transform);
        formationManager.BuildFormation(enemieTypes);
    }
    void KillFormation(FormationDead info)
    {
//        SpawnFormation(formations[Random.Range(0, formations.Length)], new []{"Trident"});
    }
}