using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shot
{
    public string type;
    public int level;
    public GameObject[] shotLevelTransforms;

    public void LevelUp()
    {
        if (level < shotLevelTransforms.Length)
        {
            level++;
            SetLevel(level);
        }
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < shotLevelTransforms.Length; i++)
        {
            if (i <= level)
            {
                if (shotLevelTransforms[i] != null && !shotLevelTransforms[i].activeSelf)
                {
                    shotLevelTransforms[i].SetActive(true);
                }
            }
            else
            {
                shotLevelTransforms[i].SetActive(false);
            }
        }
    }
}
