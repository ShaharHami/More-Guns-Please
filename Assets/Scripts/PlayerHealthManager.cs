using ForceShield;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealthManager : MonoBehaviour, IDamagable, ICauseDamage
{
    public int Health;
    public int health { get; set; }
    public int maxHealth { get; set; }
    public int damageAmount { get; set; }
    public int DamageAmount;
    public HealthDisplay healthDisplay;
    [FormerlySerializedAs("ForceShield")] public ForceShieldController forceShield;
    private int storePlayerHealth;
    private Player player;

    private void Awake()
    {
        storePlayerHealth = health = Health;
        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        damageAmount = DamageAmount;
        healthDisplay.SetHealth(storePlayerHealth, false);
    }
    public void Damage(int damage)
    {
        if (forceShield.gameObject.activeSelf) return;
        health -= damage;
        healthDisplay.ChangeHealth(health);
        if (health <= 0)
        {
            health = 0;
            player.KillPlayer();
            health = storePlayerHealth;
        }
    }
}
