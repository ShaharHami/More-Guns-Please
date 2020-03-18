using UnityEngine;

[System.Serializable]
public class Shot
{
    public string type;
    public int level;
    public GameObject[] shotLevelTransforms;
    public Player player;

    public void LevelUp()
    {
        if (level < shotLevelTransforms.Length)
        {
            SetLevel(level + 1);
        }
    }

    public void SetLevel(int manualLevel)
    {
        if (manualLevel > shotLevelTransforms.Length) return;
        if (manualLevel < level)
        {
            foreach (GameObject t in shotLevelTransforms)
            {
                t.SetActive(false);
            }
            level = 0;
        }
        for (int i = level; i < manualLevel; i++)
        {
            if (shotLevelTransforms[i] != null)
            {
                shotLevelTransforms[i].SetActive(true);
            }
        }
        level = manualLevel;

        if (Input.touchCount <= 0 && !Input.GetMouseButton(0)) return;
        if (player == null)
        {
            player = GameObject.FindObjectOfType<Player>();
        }

        player.lastFrameShooting = false;
    }

    public void SetInactive()
    {
        foreach (GameObject obj in shotLevelTransforms)
        {
            obj.SetActive(false);
        }
    }
}