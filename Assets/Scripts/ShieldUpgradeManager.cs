using UnityEngine;

namespace ForceShield
{
    public class ShieldUpgradeManager : MonoBehaviour, IUpgradable
    {
        public string moduleName;
        public string ModuleName { get; private set; }
        public bool MaxedOut { get; set; }
        public ForceShieldController forceShieldController;

        private void Awake()
        {
            ModuleName = moduleName;
            MaxedOut = forceShieldController.health >= forceShieldController.maxHealth;
        }

        public void UpgradeCount(float n)
        {
            forceShieldController.shieldMaxHealth = forceShieldController.maxHealth += (int) n;
            forceShieldController.healthDisplay.SetHealth(forceShieldController.maxHealth);
            MaxedOut = forceShieldController.health >= forceShieldController.maxHealth;
        }

        public void UpgradeHealth(float n)
        {
            if (forceShieldController.health + n >= forceShieldController.maxHealth)
            {
                n = forceShieldController.maxHealth - forceShieldController.health;
            }

            forceShieldController.Damage((int) -n);
            if (forceShieldController.shieldLevel >= 0)
            {
                forceShieldController.EnableShield();
            }
            MaxedOut = forceShieldController.health >= forceShieldController.maxHealth;
        }
    }
}