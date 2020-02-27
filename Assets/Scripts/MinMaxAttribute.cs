using UnityEngine;

// ! -> Put this anywhere, but not inside an Editor Folder
// By @febucci : https://www.febucci.com/
public class MinMaxAttribute : PropertyAttribute
{
    public float minLimit = 0;
    public float maxLimit = 1;

    public MinMaxAttribute(float min, float max)
    {
        minLimit = min;
        maxLimit = max;
    }
}