using System;
using System.Collections;
using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleFormationManager : MonoBehaviour
{
    [HideInInspector] public Formation formation;
    private Animator animator;
    private int spawnedEnemies;
    private int deadEnemies;
    private List<GameObject> positionMarkers;
    private GameObject formationContainer;

    private void Awake()
    {
        positionMarkers = new List<GameObject>();
    }
    private void OnEnable()
    {
        formation = null;
        positionMarkers.Clear();
        transform.SetParent(null);
        spawnedEnemies = 0;
        deadEnemies = 0;
        EnemyDied.RegisterListener(ClearFormation);
        ReachedPoint.RegisterListener(ParentEnemy);
    }

    private void OnDisable()
    {
        EnemyDied.UnregisterListener(ClearFormation);
        ReachedPoint.UnregisterListener(ParentEnemy);
    }
    public void BuildFormation(string[] enemieTypes)
    {
        if (formation == null) return;
        formationContainer = ObjectPooler.Instance.SpawnFromPool("FormationContainer", Vector3.zero, Quaternion.identity);
        formationContainer.transform.SetParent(transform);
        formationContainer.name = formation.formationName+"_"+formation.triggerName;
        foreach (var pos in formation.positions)
        {
            GameObject positionMarker = ObjectPooler.Instance.SpawnFromPool("PositionMarker", pos, Quaternion.identity);
            positionMarker.name = pos.ToString();
            positionMarker.transform.SetParent(formationContainer.transform);
            positionMarker.transform.localPosition = pos;
            positionMarkers.Add(positionMarker);
        }
        AnimateFormation(formation.triggerName);
        StartCoroutine(PopulateFormation(enemieTypes));
    }
    IEnumerator PopulateFormation(string[] enemieTypes)
    {
        yield return null;
        foreach (GameObject positionMarker in positionMarkers)
        {
            SpawnEnemy(enemieTypes[Random.Range(0, enemieTypes.Length)], 
                positionMarker,
                Quaternion.Euler(0, 180f, 0));
            yield return new WaitForSeconds(Random.Range(formation.spawnDelay.x, formation.spawnDelay.y));
        }
    }
    private void AnimateFormation(string trigger)
    {
        animator = formationContainer.GetComponent<Animator>();
        if (trigger != "")
        {
            animator.SetTrigger(trigger);
        }
    }
    public void SpawnEnemy(string enemyType, GameObject positionMarker, Quaternion rot)
    {
        Enemy enemy = ObjectPooler.Instance.SpawnFromPool(enemyType, Vector3.zero, rot).GetComponent<Enemy>();
        if (formation.spawnPoints.Length > 0)
        {
            enemy.transform.position = formation.spawnPoints[Random.Range(0, formation.spawnPoints.Length)];
        }
        else
        {
            enemy.transform.position = positionMarker.transform.position;
        }
        enemy.FlyToPoint(positionMarker.transform);
        spawnedEnemies++;
    }
    private void ParentEnemy(ReachedPoint info)
    {
        if (info.parentTransform.parent != formationContainer.transform) return;
        info.objTransform.SetParent(info.parentTransform);
    }

    private void ClearFormation(EnemyDied info)
    {
        if (info.point.parent != formationContainer.transform)
        {
            return;
        }
        deadEnemies++;
        Enemy e = info.enemy.GetComponent<Enemy>();
        info.point.SetParent(null);
        info.point.gameObject.SetActive(false);
        info.enemy.SetParent(null);
        info.enemy.gameObject.SetActive(false);
        if (spawnedEnemies == formation.positions.Length && spawnedEnemies == deadEnemies)
        {
            formationContainer.transform.SetParent(null);
            formationContainer.SetActive(false);
            transform.SetParent(null);
            gameObject.SetActive(false);
            FormationDead formationDead = new FormationDead();
            formationDead.FireEvent();
        }
    }
}