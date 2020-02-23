using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Formation", menuName = "Create Formation")]
public class Formation : ScriptableObject
{
    public string formationName;
    public string triggerName;
    [Header("Formation Positions")]
    public Vector3[] positions;
    [Header("Offset")]
    public Vector3 initialPosition;
}