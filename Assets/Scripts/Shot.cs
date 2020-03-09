using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
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

    public void ResetLevel()
    {
        
    }
    public void SetLevel(int manualLevel)
    {
        foreach (GameObject t in shotLevelTransforms)
        {
            t.SetActive(false);
        }
        level = manualLevel;
        if (level > shotLevelTransforms.Length) return;
        for (int i = 0; i < level; i++)
        {
            if (shotLevelTransforms[i] != null)
            {
                shotLevelTransforms[i].SetActive(true);
            }
        }
    }

    public void SetInactive()
    {
        foreach (GameObject obj in shotLevelTransforms)
        {
            obj.SetActive(false);
        }
    }
}