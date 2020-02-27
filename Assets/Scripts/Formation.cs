using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Formation", menuName = "Create Formation")]
public class Formation : ScriptableObject
{
    public Animator animator;
    public string formationName;
    [HideInInspector] public string triggerName;
    [HideInInspector] public int triggerIdx;
    [Header("Formation Positions")]
    public Vector3[] positions;
    [MinMax(0f, 1.2f)]
    public Vector2 spawnDelay;
    [Header("Spawn Points")]
    [Tooltip("Enemies will randomly spawn from these points. If left empty enemies will spawn in the original formation positions.")]
    public Vector3[] spawnPoints;
    [Header("Offset")]
    [MinMax(-150f, 150f)]
    public Vector2 initialPositionX;
    [MinMax(-150f, 150f)]
    public Vector2 initialPositionZ;
}