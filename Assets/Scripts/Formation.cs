using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Create Formation")]
public class Formation : ScriptableObject
{
    public Vector3[] positions;
    public string formationName;
    public Vector3 initialPosition;
}