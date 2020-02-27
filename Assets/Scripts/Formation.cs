using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Create Formation")]
public class Formation : ScriptableObject
{
    public Animator animator;
    public string formationName;
    [HideInInspector] public string triggerName;
    [HideInInspector] public int triggerIdx;
    [Header("Formation Positions")]
    public Vector3[] positions;
    [Header("Offset")]
    public Vector3 initialPosition;
}