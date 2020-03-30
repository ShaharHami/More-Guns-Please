using EventCallbacks;
using UnityEngine;
[RequireComponent(typeof(Volley))]
public abstract class ShootingPoints : MonoBehaviour, IUpgradable
{
    public string ModuleName { get; private set; }

    public string name;
    public bool enabled;
    public Volley volley;
    [Header("Gizmo Colors")] 
    public Color lineColor;
    public Color pointColor;
    public int maxShootingPoints;
    public bool MaxedOut { get; protected set; }
    public abstract Vector3[] GetShootingPoints();
    protected abstract void UpgradeChildCount(float n);
    private bool isShooting;
    
    private void Update()
    {
        if (volley != null)
        {
            volley.SetShootingPoints(GetShootingPoints());
        }
    }

    private void OnEnable()
    {
        UpgradeEvent.RegisterListener(OnUpgrade);
    }

    private void OnUpgrade(UpgradeEvent upgradeEvent)
    {
        if (volley != null)
        {
            volley.SetShootingPoints(GetShootingPoints());
        }
    }

    private void Awake()
    {
        ModuleName = name;
        FireWeapon.RegisterListener(SetShooting);
    }

    private void SetShooting(FireWeapon fireWeaponEvent)
    {
        isShooting = fireWeaponEvent.fire;
    }
    public void UpgradeCount(float n)
    {
        UpgradeChildCount(n);
        if (!isShooting) return;
        FireWeapon haltWeapon = new FireWeapon {fire = false};
        haltWeapon.FireEvent();
        Invoke(nameof(RestartWeapons), 0.5f);
    }

    void RestartWeapons()
    {
        FireWeapon fireWeapon = new FireWeapon {fire = true};
        fireWeapon.FireEvent();
    }
}
