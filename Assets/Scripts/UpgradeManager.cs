using System.Collections.Generic;
using System.Linq;
using EventCallbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> upgrades;
    public static UpgradeManager Instance;
    public float spawnOffset;
    private List<IUpgradable> upgradables;
    private Camera camera;
    private List<GameObject> activeUpgrades;

    void Awake()
    {
        Instance = this;
        camera = Camera.main;
        activeUpgrades = new List<GameObject>();
    }
    private void OnEnable()
    {
        UpgradeEvent.RegisterListener(Upgrade);
    }

    private void OnDisable()
    {
        UpgradeEvent.UnregisterListener(Upgrade);
    }

    void Upgrade(UpgradeEvent upgradeEvent)
    {
        if (upgradables == null || upgradables.Count <= 0) return;
        foreach (var upgradable in upgradables.Where(upgradable => upgradeEvent.moduleName == upgradable.ModuleName))
        {
            switch (upgradeEvent.propName)
            {
                case "count":
                {
                    upgradable.UpgradeCount(upgradeEvent.upgradeValue);
                    break;
                }
                case "health":
                {
                    upgradable.UpgradeHealth(upgradeEvent.upgradeValue);
                    break;
                }
            }
        }

        foreach (var activeUpgrade in activeUpgrades)
        {
            if (!activeUpgrade.activeSelf) return;
            activeUpgrade.SetActive(false);
        }
        activeUpgrades = new List<GameObject>();
    }
    private void Start()
    {
        // upgradables = FindObjectsOfType<MonoBehaviour>().OfType<IUpgradable>().ToList();
    }

    public void SpawnUpgrades(int upgradeCount)
    {
        upgradables = FindObjectsOfType<MonoBehaviour>().OfType<IUpgradable>().ToList();
        List<Upgrade> ugds = new List<Upgrade>();
        foreach (var upgradable in upgradables.Where(upgradable => !upgradable.MaxedOut))
        {
            ugds.AddRange(upgrades.Where(up => up.moduleName == upgradable.ModuleName));
        }
        Vector3 screen =
            camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, camera.transform.position.y));
        float margin = (screen.x * 2) / upgradeCount;
        float t = upgradeCount / 2f;
        for (int i = 0; i < upgradeCount; i++)
        {
            GameObject upgrade = ObjectPooler.Instance.SpawnFromPool(ugds[Random.Range(0, ugds.Count)].name, new Vector3((margin * (i+.5f-t)), 0, screen.z + spawnOffset), Quaternion.Euler(new Vector3(0,180,0)));
            activeUpgrades.Add(upgrade);
        }
    }
}
